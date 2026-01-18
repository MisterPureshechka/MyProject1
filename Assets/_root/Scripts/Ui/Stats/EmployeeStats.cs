using System;
using System.Collections.Generic;
using Scripts.EmployeeLogic;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class EmployeeStats : MonoBehaviour
    {
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private EmployeeStatInfo _statInfoPrefab;
        
        private List<EmployeeStatInfo> _stats = new();
        private Company _company;

        public void Init(Company company)
        {
            _company = company;
            _company.OnEmployeeAdded += AddStat;
            Debug.LogError($"init");
            LoadStats();
        }

        private void LoadStats()
        {
            Debug.LogError($"Has to work here" + _company.Employees.Count + "- count");
            foreach (var employee in _company.Employees)
            {
                Debug.LogError($"{employee.Name} added");
                AddStat(employee);
            }
        }

        private void AddStat(Employee employee)
        {
            var statInstance = Instantiate(_statInfoPrefab, _statsContainer);
            statInstance.Init(employee);
            _stats.Add(statInstance);
        }
        
        private void OnDestroy()
        {
            _company.OnEmployeeAdded -= AddStat;
            _stats?.Clear();
        }
    }
}