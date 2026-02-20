using System;
using System.Collections.Generic;
using Scripts.EmployeeLogic;
using UnityEngine;

namespace Scripts.Ui.EmployeeShop
{
    public class EmployeeShopView : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private EmployeeShopItem _shopItemPrefab;
        
        private Dictionary<string, EmployeeShopItem> _employeeMap = new();
        
        public Action<Employee, int> OnEmployeePurchased;
        
        public void AddItem(Employee employee, int price)
        {
            var instance = Instantiate(_shopItemPrefab, _container);
            instance.Init(employee, price);
            instance.BuyButton.onClick.AddListener(() =>
            {
                OnEmployeePurchased?.Invoke(employee, price);
            });
            
            _employeeMap.Add(employee.Id, instance);
        }

        public void Destroy(string id)
        {
            var item = _employeeMap[id];
            item.Destroy();
            _employeeMap.Remove(id);
        }
    }
}