using System;
using System.Collections.Generic;
using Core;
using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Scripts.Ui.EmployeeShop
{
    public class EmployeeShopLogic : ICleanUp
    {
        private readonly Company _company;
        private EmployeeShopView _employeeShop;
        private RoomLogic _roomLogic;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;

        private int _shopItemCount = 2;
        private readonly SaveService _saveService;

        public EmployeeShopLogic(
            Company company,
            EmployeeShopView employeeShop,
            RoomLogic roomLogic,
            ProgressDataAdapter progressDataAdapter,
            SaveService saveService,
            LocalEvents localEvents)
        {
            _company = company;
            _employeeShop = employeeShop;
            _roomLogic = roomLogic;
            _progressDataAdapter = progressDataAdapter;
            _saveService = saveService;
            _localEvents = localEvents;

            LoadShop();
        }

        private void LoadShop()
        {
            var shopData = _progressDataAdapter.Data.EmployeeShop;

            // 1) Если уже есть сохранённые офферы — рисуем их
            if (shopData.Offers != null && shopData.Offers.Count > 0)
            {
                foreach (var offer in shopData.Offers)
                {
                    var employee = RestoreEmployeeFromOffer(offer);
                    _employeeShop.AddItem(employee, offer.Price);
                }

                _employeeShop.OnEmployeePurchased += EmployeePurchaseListener;
                return;
            }

            // 2) Иначе генерим, сохраняем и рисуем
            shopData.Offers = new List<EmployeeOfferSave>(_shopItemCount);

            for (int i = 0; i < _shopItemCount; i++)
            {
                float power01 = 0f; // позже можешь брать из прогресса/майлстоуна
                var employee = GenerateEmployee(power01);
                var price = Calculate(employee.ExportSkills(), 100);

                _employeeShop.AddItem(employee, price);

                shopData.Offers.Add(ToOfferSave(employee, price));
            }

            _saveService.SaveProgress(_progressDataAdapter.Data);

            _employeeShop.OnEmployeePurchased += EmployeePurchaseListener;
        }
        
        private void EmployeePurchaseListener(Employee employee, int price)
        {
            var currentAmount = _progressDataAdapter.Data.Money;
            if (currentAmount < price) return;

            if (_roomLogic.Room.TryGetRandomFreeSlotIndex(out var roomItemSlot))
            {
                _roomLogic.PlaceItem(roomItemSlot, employee);
                _employeeShop.Destroy(employee.Id);
                _company.AddEmployee(employee, roomItemSlot);

                // ✅ удалить из сохранённых офферов
                var offers = _progressDataAdapter.Data.EmployeeShop.Offers;
                offers.RemoveAll(o => o.Id == employee.Id);

                _saveService.SaveProgress(_progressDataAdapter.Data);
            }
            else
            {
                Debug.LogError("No free space");
                return;
            }

            currentAmount -= price;
            _progressDataAdapter.Data.Money = currentAmount;
            _localEvents.TriggerWalletUpdate(currentAmount);
        }
        
        private EmployeeOfferSave ToOfferSave(Employee employee, int price)
        {
            var offer = new EmployeeOfferSave
            {
                Id = employee.Id,
                Name = employee.Name, // если у тебя есть Name; если нет — убери
                Price = price
            };

            foreach (var kv in employee.Skills)
            {
                offer.Skills.Add(new SkillSave
                {
                    Key = kv.Key.ToString(),   // DevTaskType в string
                    Value = kv.Value
                });
            }

            return offer;
        }

        private Employee RestoreEmployeeFromOffer(EmployeeOfferSave offer)
        {
            var employee = new Employee(offer.Id, string.IsNullOrEmpty(offer.Name) ? "John Doe" : offer.Name);

            var map = new Dictionary<string, float>(offer.Skills.Count);
            for (int i = 0; i < offer.Skills.Count; i++)
            {
                map[offer.Skills[i].Key] = offer.Skills[i].Value;
            }

            employee.ImportSkills(map);
            return employee;
        }



        private Employee GenerateEmployee(float power01)
        {
            power01 = Mathf.Clamp01(power01);

            var employee = new Employee(Guid.NewGuid().ToString("N"), "John Doe");

            float chanceSecondSkill = Mathf.Lerp(0f, 0.08f, power01); // максимум 8%
            float chanceThirdSkill  = Mathf.Lerp(0f, 0.01f, power01); // максимум 1%

            int skillCount = 1;
            if (UnityEngine.Random.value < chanceSecondSkill) skillCount = 2;
            if (skillCount == 2 && UnityEngine.Random.value < chanceThirdSkill) skillCount = 3;

            var allTypes = (DevTaskType[])System.Enum.GetValues(typeof(DevTaskType));
            var used = new HashSet<int>();
            var skillMap = new Dictionary<string, float>(skillCount);

            while (skillMap.Count < skillCount && used.Count < allTypes.Length)
            {
                int idx = UnityEngine.Random.Range(0, allTypes.Length);
                if (!used.Add(idx)) continue;

                var type = allTypes[idx];

                float value = RollSkillValue(power01);

                value = Mathf.Clamp(value, 1f, 3f);

                skillMap[type.ToString()] = value;
            }

            employee.ImportSkills(skillMap);
            return employee;
        }

        private float RollSkillValue(float power01)
        {
            if (power01 <= 0f)
                return 1f;

            float t = Mathf.Pow(UnityEngine.Random.value, 2.8f); // bias к 0

            float max = Mathf.Lerp(1.2f, 3.0f, power01);

            float raw = Mathf.Lerp(1f, max, t);

            int stepped = Mathf.Clamp(Mathf.RoundToInt(raw), 1, 3);
            return stepped;
        }

        
        private int Calculate(
            IReadOnlyDictionary<string, float> skills,
            int basePrice,
            float skillCap = 10f,
            float randomSpread01 = 0.05f,
            int minPrice = 1)
        {
            if (skills == null) return Math.Max(minPrice, basePrice);

            float sum = 0f;

            foreach (var kv in skills)
            {
                var skillName  = kv.Key;
                var skillValue = kv.Value;

                float x = Math.Clamp(skillValue / skillCap, 0f, 1f);

                float curved = x * x; 

                sum += curved;
            }

            float price = basePrice + sum;

            if (randomSpread01 > 0f)
            {
                float r = (float)(new Random().NextDouble() * 2.0 - 1.0); 
                price *= 1f + r * randomSpread01;
            }

            return Math.Max(minPrice, (int)MathF.Round(price));
        }

        public void CleanUp()
        {
            Object.Destroy(_employeeShop.gameObject);
        }
    }
}