using System;
using UnityEngine;

namespace Scripts.Ui.OfficeShop
{
    public class OfficeShopView : MonoBehaviour
    { 
        [SerializeField] private Transform _container;
        [SerializeField] private OfficeShopItem _officeShopItem;
        public Transform Container => _container;
        
        public Action<OfficeOffer> OnItemSelected;

        public void Add(OfficeOffer offer)
        {
            var instance = Instantiate(_officeShopItem, _container);
            instance.Init(offer);
            instance.Button.onClick.AddListener(() => OnItemSelected?.Invoke(offer));
        }

        private void ButtonListener(OfficeOffer offer)
        {
            OnItemSelected?.Invoke(offer);
        }
    }
}