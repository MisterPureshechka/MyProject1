using System;
using System.Collections.Generic;
using Core;
using Scripts.Progress;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class Company : IController
    {
        private readonly EmployeeFactory _employeeFactory;
        private readonly RoomLogic _roomLogic;
        private readonly SaveService _saveService;
        private readonly ProgressDataAdapter _progress;

        private readonly List<Employee> _employees = new();
        
        public IReadOnlyList<Employee> Employees => _employees;
        public event Action<Employee> OnEmployeeAdded;
        public event Action<Employee> OnEmployeeTick;

        public Company(
            EmployeeFactory employeeFactory,
            RoomLogic roomLogic,
            SaveService saveService,
            ProgressDataAdapter progress)
        {
            _employeeFactory = employeeFactory;
            _roomLogic = roomLogic;
            _saveService = saveService;
            _progress = progress;
            LoadFromProgress();
        }

        public void AddEmployee(Employee employee, int column)
        {
            _employees.Add(employee);

            employee.OnSkillsChanged += EmployeeSkillsChanged;

            UpsertEmployeeProgress(employee, column);

            _saveService.SaveProgress(_progress.Data);

            OnEmployeeAdded?.Invoke(employee);
        }
        
        private void EmployeeSkillsChanged(Employee e)
        {
            var ep = FindEmployeeProgress(e.Id);
            int column = ep != null ? ep.Column : 0;

            UpsertEmployeeProgress(e, column);

            _saveService.SaveProgress(_progress.Data);
        }

        private void UpsertEmployeeProgress(Employee e, int column)
        {
            var data = _progress.Data;
            data.Employees ??= new List<EmployeeProgressData>();

            var ep = FindEmployeeProgress(e.Id);
            if (ep == null)
            {
                ep = new EmployeeProgressData { Id = e.Id };
                data.Employees.Add(ep);
            }

            ep.Name = e.Name;
            ep.Column = column;
            ep.Skills = e.ExportSkills();
        }

        private EmployeeProgressData FindEmployeeProgress(string id)
        {
            var list = _progress.Data.Employees;
            if (list == null) return null;

            for (int i = 0; i < list.Count; i++)
                if (list[i].Id == id) return list[i];

            return null;
        }


        private void OnEmployeeWorkTick(Employee employee)
        {
            OnEmployeeTick?.Invoke(employee);
        }

        private void LoadFromProgress()
        {
            ProgressData progress = _saveService.LoadProgress();
            
            if (progress == null || progress.Employees == null || progress.Employees.Count == 0)
                return;

            foreach (var e in progress.Employees)
            {
                if (_employees.Exists(x => x.Id == e.Id))
                    continue;

                var employee = _employeeFactory.CreateEmployee(e.Id, e.Name);
                employee.ImportSkills(e.Skills);

                _employees.Add(employee);
                OnEmployeeAdded?.Invoke(employee);

                _roomLogic.PlaceItem(e.Column, employee);
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var employee in _employees)
                employee.Update(deltaTime, OnEmployeeWorkTick);
        }
    }
}
