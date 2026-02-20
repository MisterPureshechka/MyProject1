using Core;
using Scripts.EmployeeLogic;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class EmployeeStatController : ICleanUp
    {
        private Company _company;
        private EmployeeStats _employeeStats;

        public EmployeeStatController(Company company, EmployeeStats employeeStats)
        {
            _company = company;
            _employeeStats = employeeStats;
            
            _employeeStats.Init(_company);
            _company.OnEmployeeAdded += EmployeeAddedListener;
        }

        private void EmployeeAddedListener(Employee employee)
        {
            _employeeStats.AddStat(employee);
        }

        public void CleanUp()
        {
            _company.OnEmployeeAdded -= EmployeeAddedListener;
            Object.Destroy(_employeeStats.gameObject);
        }
    }
}