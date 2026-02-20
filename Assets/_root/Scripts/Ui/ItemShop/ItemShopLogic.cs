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

            // 1) Если уже сохранён оффер — показываем его
            if (shopData.OfferIds != null && shopData.OfferIds.Count > 0)
            {
                // гарантируем ровно 1 (на случай старых сохранений)
                var id = shopData.OfferIds[0];
                var cfg = _roomItemData.GetById(id);
                if (cfg != null)
                    _shopView.AddItem(cfg);

                _shopView.OnItemPurchased += ItemPurchaseListener;
                return;
            }

            // 2) Иначе выбираем один случайный и сохраняем
            int index = Random.Range(0, _roomItemConfigs.Count);
            var randomItem = _roomItemConfigs[index];

            _shopView.AddItem(randomItem);

            shopData.OfferIds = new List<string>(1) { randomItem.Id };
            _saveService.SaveProgress(_progressDataAdapter.Data);

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
                _progressDataAdapter.Data.Money -= config.Cost;
                _localEvents.TriggerWalletUpdate(_progressDataAdapter.Data.Money);
            }
            else
            {
                Debug.LogError($"No free space");  
            }
        }
        
        public void CleanUp()
        {
            Object.Destroy(_shopView.gameObject);
            _shopView.OnItemPurchased -= ItemPurchaseListener;
        }
    }
}