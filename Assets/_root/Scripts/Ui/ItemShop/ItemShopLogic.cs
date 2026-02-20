using System.Collections.Generic;
using System.Threading.Tasks;
using _root.Scripts.Rooms.RoomItems;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Rooms.RoomItems;
using Scripts.Rooms.SlotLogic;
using UnityEngine;

namespace Scripts.Ui.ItemShop
{
    public class ItemShopLogic : ICleanUp
    {
        private RoomItemDatabase _roomItemData;
        private ItemShopView _shopView;
        private readonly RoomLogic _roomLogic;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly SaveService _saveService;
        private readonly LocalEvents _localEvents;

        private IReadOnlyList<RoomItemConfig> _roomItemConfigs;
        
        // Храним текущий оффер мебели для возможности сохранения
        private RoomItemConfig _currentOffer;

        public ItemShopLogic(RoomItemDatabase roomItemData, ItemShopView shopView, RoomLogic roomLogic, ProgressDataAdapter progressDataAdapter, SaveService saveService, LocalEvents localEvents)
        {
            _roomItemData = roomItemData;
            _shopView = shopView;
            _roomLogic = roomLogic;
            _progressDataAdapter = progressDataAdapter;
            _saveService = saveService;
            _localEvents = localEvents;

            _roomItemConfigs = _roomItemData.All;
            LoadShop();
        }

        private void LoadShop()
        {
            if (_roomItemConfigs == null || _roomItemConfigs.Count == 0)
                return;

            var shopData = _progressDataAdapter.Data.CurrentShopFurniture;

            // 1) Если есть сохраненный оффер - загружаем его
            if (shopData.OfferIds != null && shopData.OfferIds.Count > 0)
            {
                var id = shopData.OfferIds[0];
                var cfg = _roomItemData.GetById(id);
                if (cfg != null)
                {
                    _shopView.AddItem(cfg);
                    _currentOffer = cfg;
                }

                _shopView.OnItemPurchased += ItemPurchaseListener;
                return;
            }

            // 2) Иначе выбираем случайный предмет мебели (но НЕ сохраняем его)
            int index = Random.Range(0, _roomItemConfigs.Count);
            var randomItem = _roomItemConfigs[index];

            _shopView.AddItem(randomItem);
            _currentOffer = randomItem;

            _shopView.OnItemPurchased += ItemPurchaseListener;
        }

        private void ItemPurchaseListener(string id)
        {
            var money = _progressDataAdapter.Data.Money;
            var config = _roomItemData.GetById(id);
            if (money < config.Cost)
            {
                Debug.LogError($"Not enough money");
                return;
            }
            
            if (_roomLogic.Room.TryGetRandomFreeSlotIndex(out var roomItemSlot))
            {
                _roomLogic.PlaceItem(roomItemSlot, new RoomItem(config));
                _shopView.Destroy(id);
                _currentOffer = null; // Обнуляем купленный оффер
                _progressDataAdapter.Data.Money -= config.Cost;
                _localEvents.TriggerWalletUpdate(_progressDataAdapter.Data.Money);
            }
            else
            {
                Debug.LogError($"No free space");  
            }
        }
        public void SaveCurrentOffer()
        {
            var shopData = _progressDataAdapter.Data.CurrentShopFurniture;
            
            if (_currentOffer != null)
            {
                shopData.OfferIds = new List<string>(1) { _currentOffer.Id };
            }
            else
            {
                shopData.OfferIds?.Clear();
            }
        }
        
        
        public void CleanUp()
        {
            Object.Destroy(_shopView.gameObject);
            _shopView.OnItemPurchased -= ItemPurchaseListener;
        }
    }
}