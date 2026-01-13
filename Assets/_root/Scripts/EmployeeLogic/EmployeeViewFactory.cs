using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class EmployeeViewFactory
    {
        private readonly EmployeeItemView _employeeItemPrefab;

        public EmployeeViewFactory(EmployeeItemView employeeItemPrefab)
        {
            _employeeItemPrefab = employeeItemPrefab;
        }

        public EmployeeItemView Create(Employee employee, Transform parent)
        {
            if (_employeeItemPrefab == null)
            {
                Debug.LogError("Employee prefab is not assigned!");
                return null;
            }

            var employeeView = Object.Instantiate(_employeeItemPrefab, parent).GetComponent<EmployeeItemView>();
            employeeView.Init(employee);

            return employeeView;
        }
    }
}