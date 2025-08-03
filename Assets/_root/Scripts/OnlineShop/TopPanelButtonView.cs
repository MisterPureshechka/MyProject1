using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.OnlineShop
{
    public class TopPanelButtonView : MonoBehaviour
    {
        [field: SerializeField] public Button TopPanelButton;
        [SerializeField] private TextMeshProUGUI _buttonText;
        
        public Action<ShopItemType> OnClick;
        private ShopItemType _shopItemType;

        public void SetButtonText(ShopItemType shopItemType)
        {
            _shopItemType = shopItemType;
            _buttonText.text = shopItemType.ToString();
            
            TopPanelButton.onClick.AddListener(() => ButtonListener(shopItemType));
        }

        private void ButtonListener(ShopItemType shopItemType)
        {
            OnClick?.Invoke(shopItemType);
        }

        public void SetButtonActive(bool isActive)
        {
            _buttonText.color = isActive ? Color.white : Color.gray;
        }

        private void OnDestroy()
        {
            TopPanelButton.onClick.RemoveAllListeners();
        }
    }
}