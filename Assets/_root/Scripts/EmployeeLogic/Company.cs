using System;
using System.Collections.Generic;
using Core;
using Scripts.Progress;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public class Company : IExecute
    {
        private readonly EmployeeFactory _employeeFactory;
        private readonly SprintSystem _sprintSystem;
        private readonly RoomLogic _roomLogic;
        private readonly GameProgress _gameProgress;

        private readonly List<Employee> _employees = new();

        public IReadOnlyList<Employee> Employees => _employees;
        public event Action<Employee> OnEmployeeAdded;

        public Company(
            EmployeeFactory employeeFactory,
            SprintSystem sprintSystem,
            RoomLogic roomLogic,
            GameProgress gameProgress)
        {
            _employeeFactory = employeeFactory;
            _sprintSystem = sprintSystem;
            _roomLogic = roomLogic;
            _gameProgress = gameProgress;
            
            LoadFromProgress();
        }

        public void AddEmployee(Employee employee)
        {
            _employees.Add(employee);
            OnEmployeeAdded?.Invoke(employee);
        }

        public void Execute(float deltaTime)
        {
            foreach (var employee in _employees)
                employee.Update(deltaTime, OnEmployeeWorkTick);
        }

        private void OnEmployeeWorkTick(Employee employee)
        {
            _sprintSystem.ApplyProgressToSprint(employee);
        }

        private void LoadFromProgress()
        {
            ProgressData progress = _gameProgress.LoadProgress();
            
            if (progress == null || progress.Employees == null || progress.Employees.Count == 0)
                return;

            foreach (var e in progress.Employees)
            {
                var employee = _employeeFactory.CreateEmployee(e.Id, e.Name);

                employee.ImportSkills(e.Skills);
                _employees.Add(employee);
                OnEmployeeAdded?.Invoke(employee);

                _roomLogic.PlaceItem(e.Column, employee);
            }
        }
    }
}
