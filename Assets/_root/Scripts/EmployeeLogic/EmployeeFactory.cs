using System;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class EmployeeFactory
    {
        private EmployeeItemView _employeeItemPrefab;

        public EmployeeFactory(EmployeeItemView employeeItemPrefab)
        {
            _employeeItemPrefab = employeeItemPrefab;
        }
        
        public void CreateEmployeeWithRandomSkill()
        {
            
        }

        public Employee CreateEmployee(string id, string name)
        {
            var employee = new Employee(id, name);

            return employee;
        }
    }
}