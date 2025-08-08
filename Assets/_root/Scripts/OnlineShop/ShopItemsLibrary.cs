using System;
using System.Collections.Generic;
using Scripts.Upgrade;

namespace Scripts.OnlineShop
{
    public class ShopItemsLibrary
    {
        private Dictionary<ShopItemType, List<IShopItem>> _shopItems = new();

        public ShopItemsLibrary()
        {
            foreach (ShopItemType shopItemType in Enum.GetValues(typeof(ShopItemType)))
            {
                _shopItems.Add(shopItemType, new List<IShopItem>());
            }

            CreateItems();
        }

        public void CreateItems()
        {
            var chairs = new List<IShopItem>();

            var chair = new ShopItem(UpgradeType.Chair, 0, "Chair", "Cheap Chair", 100);
            var chair2 = new ShopItem(UpgradeType.Chair,1, "Chair2", "Not cheap Chair", 250);
            
            chairs.Add(chair);
            chairs.Add(chair2);
            
            _shopItems[ShopItemType.Chairs] = chairs;
        }

        public Dictionary<ShopItemType, List<IShopItem>> ShopItems => _shopItems;

        private List<IShopItem> GetShopItems(ShopItemType shopItemType)
        {
            return ShopItems[shopItemType];
        }
    }
}