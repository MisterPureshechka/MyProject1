using Core;
using Scripts.Ui;
using UnityEngine;

namespace _root.Scripts.EmployeeLogic
{
    public class MainShopController : ICleanUp
    {
        private MainShopView _shopView;

        public MainShopController(MainShopView shopView)
        {
            _shopView = shopView;
        }
        public void CleanUp()
        {
            Object.Destroy(_shopView.gameObject);
        }
    }
}