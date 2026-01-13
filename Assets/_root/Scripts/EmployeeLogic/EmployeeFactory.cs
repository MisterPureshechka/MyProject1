using System;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class EmployeeFactory
    {
        private EmployeeItemView _employeeItemPrefab;

        public event Action<Employee> OnEmployeeCreated; 

        public EmployeeFactory(EmployeeItemView employeeItemPrefab)
        {
            _employeeItemPrefab = employeeItemPrefab;
        }
        
        public void CreateEmployeeWithRandomSkill()
        {
            
        }

        public Employee CreateEmployee(string name)
        {
            var employee = new Employee(Guid.NewGuid().ToString(), name);
            
            OnEmployeeCreated?.Invoke(employee);

            return employee;
        }
    }
}