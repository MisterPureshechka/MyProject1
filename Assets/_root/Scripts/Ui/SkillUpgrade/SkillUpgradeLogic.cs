using System;
using System.Collections.Generic;
using _root.Scripts.Rooms.RoomItems;
using Core;
using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Ui.ItemShop;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Ui.SkillUpgrade
{
    public class SkillUpgradeLogic : ICleanUp
    {
        private readonly SkillUpgradeShopView _skillUpgradeShopView;
        private readonly LocalEvents _localEvents;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly SaveService _saveService;
        private readonly Company _company;

        private int _experience;
        
        // Храним текущие офферы для сохранения порядка и пустых слотов
        private readonly List<SkillUpgradeOffer> _currentOffers = new();

        public SkillUpgradeLogic(SkillUpgradeShopView skillUpgradeShopView, LocalEvents localEvents,
            ProgressDataAdapter progressDataAdapter, SaveService saveService, Company company)
        {
            _skillUpgradeShopView = skillUpgradeShopView;
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _saveService = saveService;
            _company = company;

            _skillUpgradeShopView.Init(_localEvents);
            _experience = _progressDataAdapter.Data.Experience;

            LoadOffers();
        }

        private void UpgradePurchasedListener(SkillUpgradeOffer offer, Employee employee)
        {
            _experience = _progressDataAdapter.Data.Experience;

            if (offer.SkillUpgradeCost > _experience)
                return;

            foreach (var type in offer.SkillUpgradeMap.Keys)
                employee.AddSkill(type, offer.SkillUpgradeMap[type]);

            _experience -= offer.SkillUpgradeCost;
            _progressDataAdapter.Data.Experience = _experience;

            // Update employee skills in ProgressData
            var employeeData = _progressDataAdapter.Data.Employees.Find(e => e.Id == employee.Id);
            if (employeeData != null)
            {
                employeeData.Skills = employee.ExportSkills();
            }

            _skillUpgradeShopView.RemoveItem(offer);
            
            // Находим индекс купленного оффера и заменяем на null (пустой слот)
            for (int i = 0; i < _currentOffers.Count; i++)
            {
                if (_currentOffers[i] != null && _currentOffers[i].Id == offer.Id)
                {
                    _currentOffers[i] = null;
                    break;
                }
            }

            _saveService.SaveProgress(_progressDataAdapter.Data);
        }


        private void LoadOffers()
        {
            var shopData = _progressDataAdapter.Data.SkillUpgradeShop;

            // 1) Если есть сохраненные офферы - загружаем их
            if (shopData.Offers != null && shopData.Offers.Count > 0)
            {
                foreach (var saved in shopData.Offers)
                {
                    // Пропускаем пустые слоты (null)
                    if (saved == null)
                    {
                        _currentOffers.Add(null);
                        continue;
                    }
                    
                    var offer = RestoreOffer(saved);
                    _currentOffers.Add(offer);
                    _skillUpgradeShopView.AddOffer(offer);
                }

                _skillUpgradeShopView.OnItemPurchased += UpgradePurchasedListener;
                return;
            }

            // 2) Иначе генерируем новые офферы (но НЕ сохраняем их)
            var allTypes = (DevTaskType[])Enum.GetValues(typeof(DevTaskType));
            int offerCount = 3;

            for (int i = 0; i < offerCount; i++)
            {
                int directionsCount = UnityEngine.Random.Range(1, 2);
                directionsCount = Mathf.Min(directionsCount, allTypes.Length);

                var upgradeMap = new Dictionary<DevTaskType, float>(directionsCount);
                var used = new HashSet<int>();

                while (upgradeMap.Count < directionsCount)
                {
                    int index = UnityEngine.Random.Range(0, allTypes.Length);
                    if (!used.Add(index)) continue;

                    var type = allTypes[index];
                    float value = UnityEngine.Random.Range(1, 3);

                    upgradeMap[type] = value;
                }

                var offer = new SkillUpgradeOffer
                {
                    Id = Guid.NewGuid().ToString("N"),
                    SkillUpgradeMap = upgradeMap,
                    SkillUpgradeCost = Calculate(upgradeMap, 1, 1)
                };

                _currentOffers.Add(offer);
                _skillUpgradeShopView.AddOffer(offer);
            }

            _skillUpgradeShopView.OnItemPurchased += UpgradePurchasedListener;
        }

        private SkillUpgradeOfferSave ToSave(SkillUpgradeOffer offer)
        {
            var save = new SkillUpgradeOfferSave
            {
                Id = offer.Id,
                Cost = offer.SkillUpgradeCost,
                Upgrades = new List<SkillSave>(offer.SkillUpgradeMap.Count)
            };

            foreach (var kv in offer.SkillUpgradeMap)
            {
                save.Upgrades.Add(new SkillSave
                {
                    Key = kv.Key.ToString(),
                    Value = kv.Value
                });
            }

            return save;
        }

        private SkillUpgradeOffer RestoreOffer(SkillUpgradeOfferSave saved)
        {
            var map = new Dictionary<DevTaskType, float>(saved.Upgrades.Count);

            for (int i = 0; i < saved.Upgrades.Count; i++)
            {
                var keyStr = saved.Upgrades[i].Key;
                var value = saved.Upgrades[i].Value;

                if (Enum.TryParse(keyStr, out DevTaskType type))
                    map[type] = value;
            }

            return new SkillUpgradeOffer
            {
                Id = saved.Id,
                SkillUpgradeMap = map,
                SkillUpgradeCost = saved.Cost
            };
        }


        public static int Calculate(
            Dictionary<DevTaskType, float> upgradeMap,
            int basePrice,
            int pricePerPoint,
            bool roundUp = true,
            float randomSpread01 = 0f,
            int minPrice = 1)
        {
            if (upgradeMap == null || upgradeMap.Count == 0)
                return Mathf.Max(minPrice, basePrice);

            float total = 0f;

            foreach (var kv in upgradeMap)
            {
                var value = Mathf.Max(0f, kv.Value);

                float points = roundUp ? Mathf.Ceil(value) : value;


                total += points * pricePerPoint;
            }

            float price = basePrice + total;

            if (randomSpread01 > 0f)
            {
                float r = UnityEngine.Random.Range(-1f, 1f);
                price *= 1f + r * randomSpread01;
            }

            return Mathf.Max(minPrice, Mathf.RoundToInt(price));
        }


        public void SaveCurrentOffers()
        {
            var shopData = _progressDataAdapter.Data.SkillUpgradeShop;
            shopData.Offers = new List<SkillUpgradeOfferSave>();

            // Сохраняем все слоты, включая пустые (null)
            foreach (var offer in _currentOffers)
            {
                if (offer != null)
                {
                    shopData.Offers.Add(ToSave(offer));
                }
                else
                {
                    // Пустой слот - добавляем null
                    shopData.Offers.Add(null);
                }
            }
        }

        public void CleanUp()
        {
            _skillUpgradeShopView.OnItemPurchased -= UpgradePurchasedListener;
            Object.Destroy(_skillUpgradeShopView.gameObject);
        }
    }
}