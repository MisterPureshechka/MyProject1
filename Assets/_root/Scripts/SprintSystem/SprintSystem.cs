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
            ApplyProgressToCurrentTask(employee, 5.0f);
        }

        private void ApplyProgressToCurrentTask(Employee employee, float workDelta = 1.0f)
        {
            if (employee == null) return;
            if (employee.IsBusy) return;                
            if (employee._currentState != EmployeeState.Work) return;

            // 1) получить текущую задачу сотрудника
            _assigned.TryGetValue(employee.Id, out var task);

            // 2) если задачи нет — назначить новую из очереди
            if (task == null || task.IsCompleted || task.Progress <= 0f)
            {
                task = TryAssignNextTask(employee);
                if (task == null)
                {
                    // нечего делать — сотрудник ждёт
                    employee.PauseWork();
                    // при желании: TriggerEmployeeIdle(employee);
                    return;
                }
            }

            // 3) применить прогресс (фиксированное списание)
            // РЕКОМЕНДАЦИЯ: иметь у DevTask метод ApplyWork(amount), который уменьшает Progress.
            task.ApplyWork(workDelta);

        // 4) если задача завершилась — освободить сотрудника
        if (task.IsCompleted || task.Progress <= 0f)
        {
            _assigned.Remove(employee.Id);

                // тут твои события "таск завершён"
                // _localEvents.TriggerDevTaskComplete(task.Type);
                // _gameDevProgress.CompleteTask(...)

                // 5) опционально: сразу выдать следующую задачу (если хочешь, чтобы он не простаивал до следующего тика)
            var next = TryAssignNextTask(employee);
            if (next == null)
            {
                    employee.PauseWork();
            }

                // 6) если задач нигде не осталось — завершить “спринт/пул”
                CheckAllWorkCompleted();
            }
        }

        private IDevTask TryAssignNextTask(Employee employee)
        {
            if (_pendingTasks.Count == 0)
                return null;

            // Берём с конца (как у тебя раньше) или с начала — как удобнее
            var next = _pendingTasks[^1];
            _pendingTasks.RemoveAt(_pendingTasks.Count - 1);

            _assigned[employee.Id] = next;

            // если нужно — события/UI: задача стала InProgress у конкретного employee
            // _localEvents.TriggerTaskAssigned(employee.Id, next);

            // убедиться, что сотрудник в Work (если он был на паузе)
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