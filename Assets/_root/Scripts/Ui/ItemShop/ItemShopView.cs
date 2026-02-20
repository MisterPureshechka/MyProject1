using System;
using System.Collections.Generic;
using _root.Scripts.Rooms.RoomItems;
using UnityEngine;

namespace Scripts.Ui.ItemShop
{
    public class ItemShopView : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private ItemShopItem _shopItemPrefab;
        
        private Dictionary<string, ItemShopItem> _itemMap = new();
        
        public Action<string> OnItemPurchased;
        
        public void AddItem(RoomItemConfig roomItem)
        {
            var instance = Instantiate(_shopItemPrefab, _container);
            instance.Init(roomItem);
            instance.BuyButton.onClick.AddListener(() =>
            {
                OnItemPurchased?.Invoke(roomItem.Id);
            });
            
            _itemMap.Add(roomItem.Id, instance);
        }

        public void Destroy(string id)
        {
            var item = _itemMap[id];
            item.Destroy();
            _itemMap.Remove(id);
        }
    }
}