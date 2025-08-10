using System;
using System.Collections.Generic;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Wallet;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.OnlineShop
{
    public class OnlineShopController : ICleanUp
    {
        private OnlineShopView _view;
        private LocalEvents _localEvents;
        private PrefabDataBase _prefabData;
        private ShopItemsLibrary _shopItemsLibrary;
        private readonly WalletLogic _walletLogic;
        private OnlineShopButton _shopButton;

        public OnlineShopController(OnlineShopView view, LocalEvents localEvents, PrefabDataBase prefabData, ShopItemsLibrary shopItemsLibrary, WalletLogic walletLogic)
        {
            _view = view;
            _localEvents = localEvents;
            _prefabData = prefabData;
            _shopItemsLibrary = shopItemsLibrary;
            _walletLogic = walletLogic;
            _shopButton = Object.FindAnyObjectByType<OnlineShopButton>();
            if(_shopButton == null) Debug.LogError("Shop Button Not Found");
            _shopButton.Button.onClick.AddListener(() => _localEvents.TriggerShowCatalogue(_view));
            _view.CloseButton.onClick.AddListener(() => _localEvents.TriggerHideCatalogue(_view));

            SetTopPageButtons();
        }

        private void SetTopPageButtons()
        {
            foreach (ShopItemType shopItemType in Enum.GetValues(typeof(ShopItemType)))
            {
                var buttonInstance =
                    Object.Instantiate(_prefabData.OnlineShopTopPanelButton);
                buttonInstance.OnClick += SetPageInfo;
                _view.SetTopPanelButtons(buttonInstance, shopItemType);
            }
        }

        private void SetPageInfo(ShopItemType shopItemType)
        {
            CleanItems();
            
            List<ShopItemView> shopItems = new List<ShopItemView>();

            foreach (var shopItem in _shopItemsLibrary.ShopItems[shopItemType])
            {
                var itemInstance = Object.Instantiate(_prefabData.ShopItemView).GetComponent<ShopItemView>();
                itemInstance.SetInfo(shopItem, _prefabData.ChairSprites[shopItem.Id]);
                itemInstance.BuyButton.onClick.AddListener(() => TryBuyItem(shopItem));
                shopItems.Add(itemInstance);
            }
            
            _view.SetShopItems(shopItemType, shopItems);
        }

        private void TryBuyItem(IShopItem shopItem)
        {
            if (_walletLogic.TrySpend(shopItem.Price))
            {
                _localEvents.TriggerUpdateItem(shopItem.Id, shopItem.UpgradeType);
            }
        }

        private void ChangePage()
        {
            
        }

        private void CleanItems()
        {
            _view.CleanItemsContainer();
        }
        
        public void CleanUp()
        {
            _shopButton.Button.onClick.RemoveAllListeners();
            _view.CloseButton.onClick.RemoveAllListeners();
            _view.ResetView(); 
        }
    }
}