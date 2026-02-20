using Scripts.Ui.EmployeeShop;
using Scripts.Ui.ItemShop;
using Scripts.Ui.OfficeShop;
using Scripts.Ui.SkillUpgrade;
using UnityEngine;

namespace Scripts.Ui
{
    public class MainShopView : MonoBehaviour
    {
        public EmployeeShopView Employees;
        public SkillUpgradeShopView Skills;
        public OfficeShopView Offices;
        public ItemShopView OfficeFurniture;
    }
}