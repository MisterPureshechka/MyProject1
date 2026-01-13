using System;
using System.Collections.Generic;
using Core;
using Scripts.Tasks;

namespace Scripts.EmployeeLogic
{
    public class Company : ICleanUp, IExecute
    {
        private readonly EmployeeFactory _employeeFactory;
        private readonly SprintSystem _sprintSystem;
        private readonly List<Employee> _employees = new();
        
        public event Action<Employee> OnEmployeeAdded;

        public Company(EmployeeFactory employeeFactory, SprintSystem sprintSystem)
        {
            _employeeFactory = employeeFactory;
            _sprintSystem = sprintSystem;
            _employeeFactory.OnEmployeeCreated += AddEmployee;
        }
    
        private void AddEmployee(Employee employee)
        {
            _employees.Add(employee);
            OnEmployeeAdded?.Invoke(employee);
        }

        public void Execute(float deltaTime)
        {
            foreach (var employee in _employees)
            {
                employee.Update(deltaTime, OnEmployeeWorkTick);
            }
        }

        private void OnEmployeeWorkTick(Employee employee)
        {
            _sprintSystem.ApplyProgressToSprint();
        }

        public void CleanUp()
        {
            _employeeFactory.OnEmployeeCreated -= AddEmployee;
        }
    }
}