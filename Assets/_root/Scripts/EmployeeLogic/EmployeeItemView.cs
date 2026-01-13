using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class EmployeeItemView : MonoBehaviour
    {
        [SerializeField] private EmployeeView _employeeView;
        [SerializeField] private PCView _pcView;
        
        private Employee _employee;

        public void Init(Employee employee)
        {
            _employee = employee;
            _employee.View = this;
        }

        public PCView PCView => _pcView;
        public EmployeeView EmployeeView => _employeeView;
    }
}