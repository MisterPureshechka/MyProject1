using System;
using System.Collections.Generic;
using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Ui.SkillUpgrade
{
    public class SkillUpgradeShopView : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private SkillShopItem _shopItemPrefab;
        
        private Dictionary<string, SkillShopItem> _itemMap = new();
        private SkillShopItem _currentItem;
        
        public Action<SkillUpgradeOffer, Employee> OnItemPurchased;
        
        private LocalEvents _localEvents;
        private bool _isWaitingForEmployeeClick;

        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _localEvents.OnEmployeeClicked += EmployeeClickListener;
            
            _isWaitingForEmployeeClick = false;
        }

        private void EmployeeClickListener(Employee employee)
        {
            if(!_isWaitingForEmployeeClick || _currentItem == null) return;
            
            OnItemPurchased?.Invoke(_currentItem.SkillOffer, employee);
            
            _currentItem = null;
        }

        public void AddOffer(SkillUpgradeOffer offer)
        {
            var instance = Instantiate(_shopItemPrefab, _container);
            instance.Init(offer);
            instance.BuyButton.onClick.AddListener(() =>
            {
                _currentItem = instance;
                _isWaitingForEmployeeClick = true;
            });
            
            _itemMap.Add(offer.Id, instance);
        }

        private void Destroy(string id)
        {
            var item = _itemMap[id];
            item.Destroy();
            _itemMap.Remove(id);
        }

        private void OnDestroy()
        {
            _localEvents.OnEmployeeClicked -= EmployeeClickListener;
        }

        public void RemoveItem(SkillUpgradeOffer offer)
        {
            Destroy(offer.Id);
        }
    }
}