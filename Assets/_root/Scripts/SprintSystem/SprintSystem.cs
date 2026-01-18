using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp
    {
        private readonly SprintUI _sprintUI;
        private readonly LocalEvents _localEvents;
        private readonly SprintView _sprintView;
        
        private readonly List<IDevTask> _pendingTasks = new();
        private readonly List<IDevTask> _activeTasks = new();
        
        private readonly Dictionary<string, IDevTask> _assigned = new();
        
        private ProgressDataAdapter _progressDataAdapter;
        private ISprint _currentSprint;
        private UiFactory _uiFactory;
        
        private bool _sprintCompleted;

        public SprintSystem(SprintView sprintView, SprintUI sprintUI, LocalEvents localEvents, ProgressDataAdapter progressDataAdapter, UiFactory uiFactory)
        {
            _sprintView = sprintView;
            _sprintUI = sprintUI;
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _uiFactory = uiFactory;

            _sprintView.Init(_localEvents);
            _currentSprint = new DevSprint(10, null);
            LoadSprint();

            _localEvents.OnApplyProgressToSprint += ApplyProgressToSprint;
        }

        private void LoadSprint()
        {
            for (int i = 0; i < _currentSprint.Capacity; i++)
            {
               AddTask(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Saving System", 100f));
            }
        }
        
        private async Task AddTask(ITask task)
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();

            var clone = task.Clone();

            if (_currentSprint.TryAddTask(clone))
            {
                await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform), SprintType.Dev);
                _pendingTasks.Add((IDevTask)clone);
            }  
        }
        
        public void ApplyProgressToSprint(Employee employee)
        {
            ApplyProgressToCurrentTask(employee);
        }

        private void ApplyProgressToCurrentTask(Employee employee)
        {
            if (employee == null) return;
            if (employee.IsBusy) return;
            if (employee._currentState != EmployeeState.Work) return;
            if (_sprintCompleted) return;

            _assigned.TryGetValue(employee.Id, out var task);

            if (task == null || task.IsCompleted || task.Progress <= 0f)
            {
                task = TryAssignNextTask(employee);
                if (task == null)
                {
                    employee.PauseWork();
                    TryCompleteSprint();
                    return;
                }
            }

            task.ApplyWork(employee.GetSkill(task.Type));

            if (task.IsCompleted || task.Progress <= 0f)
            {
                _assigned.Remove(employee.Id);

                var next = TryAssignNextTask(employee);
                if (next == null)
                    employee.PauseWork();

                TryCompleteSprint();
            }
        }

        
        private void TryCompleteSprint()
        {
            if (_sprintCompleted)
                return;

            if (_pendingTasks.Count > 0) return;
            if (_assigned.Count > 0) return;

            var all = _currentSprint.GetTasks();
            for (int i = 0; i < all.Count; i++)
            {
                if (!all[i].IsCompleted)
                    return;
            }

            _sprintCompleted = true;

            _localEvents.TriggerSprintCompleted(SprintType.Dev);
        }

        private IDevTask TryAssignNextTask(Employee employee)
        {
            if (_pendingTasks.Count == 0)
                return null;

            var next = _pendingTasks[^1];
            _pendingTasks.RemoveAt(_pendingTasks.Count - 1);

            _assigned[employee.Id] = next;

            employee.ResumeWork();

            return next;
        }

        private void CheckAllWorkCompleted()
        {
            // Нет задач в очереди и никто ничего не делает
            bool anyoneHasTask = _assigned.Count > 0;
            if (_pendingTasks.Count == 0 && !anyoneHasTask)
            {
                // CompleteSprint / EndSession
                // CompleteSprint();
            }
        }


        public void CleanUp()
        {
            _localEvents.OnApplyProgressToSprint -= ApplyProgressToSprint;
        }
    }
}