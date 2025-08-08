using Scripts.Upgrade;
using UnityEngine;

namespace Scripts.OnlineShop
{
    public class ShopItem : IShopItem
    {
        public UpgradeType UpgradeType { get; }
        public int Id { get; }
        public GameObject GameObject { get; set; }
        public string Title { get; }
        public string Description { get; }
        public int Price { get; }

        public ShopItem(UpgradeType upgradeType, int id, string title, string description, int price)
        {
            UpgradeType = upgradeType;
            Id = id;
            Title = title;
            Description = description;
            Price = price;
        }
    }
}