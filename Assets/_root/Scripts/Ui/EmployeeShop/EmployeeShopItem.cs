using _root.Scripts.Ui.Stats;
using Scripts.EmployeeLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.EmployeeShop
{
    public class EmployeeShopItem : MonoBehaviour
    {
        public Button BuyButton;
        
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private Transform _skillContainer;
        [SerializeField] private Transform _previewContainer;
        [SerializeField] private SkillInfo _skillInfoPrefab;

        public void Init(Employee employee, int price)
        {
            _name.text = employee.Name;

            foreach (var skill in employee.Skills)
            {
                if (skill.Value <= 0f)
                    continue;
                
                var skillInstance = Instantiate(_skillInfoPrefab, _skillContainer); 
                skillInstance.Set(skill.Key.ToString(), skill.Value);
            }
            
            _price.text = price + "$";
        }

        public void Destroy()
        {
            BuyButton.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}