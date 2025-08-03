using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.OnlineShop
{
    public class ShopItemView : MonoBehaviour
    {
        [field: SerializeField] public Button BuyButton { get; private set; }
        
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _priceText;

        public void SetInfo(IShopItem shopItem)
        {
            shopItem.GameObject = gameObject;
            _titleText.text = shopItem.Title;
            _descriptionText.text = shopItem.Description;
            _priceText.text = shopItem.Price.ToString() + "$";
        }

    }
}