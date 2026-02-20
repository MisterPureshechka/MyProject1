using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Ui.EmployeeShop;
using Scripts.Ui.ItemShop;
using Scripts.Ui.SkillUpgrade;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Ui.OfficeShop
{
    public class OfficeShopLogic : ICleanUp
    {
        private OfficeShopView _shop;
        private ProgressDataAdapter _progressData;
        private readonly GameStateMachine _stateMachine;
        private readonly SaveService _saveService;
        private readonly LocalEvents _localEvents;
        
        // Ссылки на другие магазины для сохранения их офферов при покупке офиса
        private readonly EmployeeShopLogic _employeeShop;
        private readonly SkillUpgradeLogic _skillShop;
        private readonly ItemShopLogic _furnitureShop;

        private int _money;

        public OfficeShopLogic(OfficeShopView shop, ProgressDataAdapter progressData, GameStateMachine stateMachine, SaveService saveService, LocalEvents localEvents, EmployeeShopLogic employeeShop, SkillUpgradeLogic skillShop, ItemShopLogic furnitureShop)
        {
            _shop = shop;
            _progressData = progressData;
            _stateMachine = stateMachine;
            _saveService = saveService;
            _localEvents = localEvents;
            _employeeShop = employeeShop;
            _skillShop = skillShop;
            _furnitureShop = furnitureShop;

            _money = _progressData.Data.Money;
            
            LoadOffer();
        }

        private void LoadOffer()
        {
            var offer = new OfficeOffer
            {
                Cells = _progressData.Data.OfficeCells + 1,
                Price = 2000
            };

            offer.Description = $"New office with {offer.Cells} free cells";
            
            _shop.Add(offer);
            _shop.OnItemSelected += TryBuyItem;
        }

        private void TryBuyItem(OfficeOffer offer)
        {
            if (_money <= offer.Price) return;
            
            _progressData.Data.Money -= offer.Price;
            _localEvents.TriggerWalletUpdate(_progressData.Data.Money);
            _progressData.Data.OfficeCells = offer.Cells;
            
            // Сохраняем текущие офферы всех магазинов при покупке офиса
            _employeeShop?.SaveCurrentOffers();
            _skillShop?.SaveCurrentOffers();
            _furnitureShop?.SaveCurrentOffer();
            
            _saveService.SaveProgress(_progressData.Data);
            _stateMachine.EnterState<ShopState>();
            Debug.Log("New Office purchased - shop offers saved");
        }

        public void CleanUp()
        {
            Object.Destroy(_shop.gameObject);
        }
    }
}