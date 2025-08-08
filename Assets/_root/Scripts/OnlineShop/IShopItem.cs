using Scripts.Upgrade;
using UnityEngine;

namespace Scripts.OnlineShop
{
    public interface IShopItem
    {
        UpgradeType UpgradeType { get; }
        int Id { get; }
        GameObject GameObject { get; set; }
        string Title { get; }
        string Description { get; }
        int Price { get; }
    }
}