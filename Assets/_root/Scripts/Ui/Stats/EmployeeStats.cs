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
        }

        private void AddStat(Employee employee)
        {
            var statInstance = Instantiate(_statInfoPrefab, _statsContainer);
            statInstance.Init(employee);
            _stats.Add(statInstance);
        }
        
        private void OnDestroy()
        {
            _stats?.Clear();
        }
    }
}