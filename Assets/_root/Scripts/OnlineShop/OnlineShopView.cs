using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.OnlineShop
{
    public class OnlineShopView : MonoBehaviour, ICatalogue
    {
        [field: SerializeField] public Button CloseButton;
     
        [SerializeField] private RectTransform _shopPanel;
        [SerializeField] private Transform _topPanelButtonsContainer;
        [SerializeField] private Transform _shopItemsContainer;

        private List<ShopItemView> _shopItemViews = new();
        private Dictionary<ShopItemType, TopPanelButtonView> _topPanelButtonViewMap = new();
        private Sequence _sequence;

        private Vector2 _startPosition = new Vector2(0, -80);
        private Vector2 _hidePosition;
        private Vector2 _offset = new (0, -400);

        public Transform TopPanelButtonsContainer => _topPanelButtonsContainer;

        private void Start()
        {
            _hidePosition = _startPosition + _offset;
            _shopPanel.localPosition = _hidePosition;
            //gameObject.SetActive(false);
        }

        public void SetShopItems(ShopItemType shopItemType, List<ShopItemView> shopItemViews)
        {
            foreach (var shopItemView in shopItemViews)
            {
                shopItemView.transform.SetParent(_shopItemsContainer);
                shopItemView.transform.localScale = Vector3.one;
                _shopItemViews.Add(shopItemView);
            }

            SetActiveButton(shopItemType);
        }

        private void SetActiveButton(ShopItemType shopItemType)
        {
            foreach (var shopItemView in _topPanelButtonViewMap.Values)
            {
                shopItemView.SetButtonActive(false);
            }

            _topPanelButtonViewMap[shopItemType].SetButtonActive(true);
        }

        public void SetTopPanelButtons(TopPanelButtonView button, ShopItemType type)
        {
            button.transform.SetParent(TopPanelButtonsContainer);
            button.transform.localScale = Vector3.one;
            _topPanelButtonViewMap.Add(type, button);
            button.SetButtonText(type);
            SetActiveButton(type);
        }

        public void CleanItemsContainer()
        {
            foreach (Transform child in _shopItemsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public bool IsVisible { get; }
        
        public void Hide(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            gameObject.SetActive(true);
            _sequence.Append(_shopPanel.DOLocalMove(_hidePosition, 0.4f).SetEase(Ease.InSine));
            _sequence.OnComplete(() => onComplete?.Invoke());
        }

        private void HideOnStart()
        {
            gameObject.SetActive(false);
            transform.position = _hidePosition;
        }


        public void Show(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            gameObject.SetActive(true);
            _sequence.Append(_shopPanel.DOLocalMove(_startPosition, 0.6f).SetEase(Ease.OutSine));
            _sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}