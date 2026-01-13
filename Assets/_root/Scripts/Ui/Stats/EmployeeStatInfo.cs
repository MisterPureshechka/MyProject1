using Scripts.EmployeeLogic;
using Scripts.Progress;
using TMPro;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class EmployeeStatInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        
        [SerializeField] private StatInfo _energyInfo;
        [SerializeField] private StatInfo _moodInfo;
        [SerializeField] private StatInfo _foodInfo;
        
        private string _key; 
        private Employee _employee;
        private string _tooltip;

        public void Init(Employee employee)
        {
            _employee = employee;
            UpdateInfo();
            _employee.OnStatUpdate += UpdateInfo;
            
        }

        public void UpdateInfo()
        {
            _name.text = _employee.Name;
            _energyInfo.UpdateInfo(_employee.Energy, _employee.MaxValue);
            _moodInfo.UpdateInfo(_employee.Mood, _employee.MaxValue);
            _foodInfo.UpdateInfo(_employee.Hunger, _employee.MaxValue);
        }
    }
}