using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp
    {
        private readonly Queue<ITask> _pendingTasks = new();
        private readonly List<ITask> _activeTasks = new();
        private List<ITask> _hiddenTasks = new ();
        private bool _isSprintHidden;
        
        private int _maxSimultaneousUpdates = 2;

        private readonly AllTaskView _allTaskView;
        private readonly TaskLibrary _taskLibrary;
        private readonly SprintView _sprintView;
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;

        private DevSprint _devSprint;
        private EatSprint _eatSprint;
        private ChillSprint _chillSprint;
    
        private SprintBase _currentSprint;
        
        public SprintSystem(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents)
        {
            _taskLibrary = taskLibrary;
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _allTaskView = _uiFactory.GetAllTaskView(canvas.transform);
            _allTaskView.Init(_localEvents);
            
            _devSprint = new DevSprint(12);
            _eatSprint = new EatSprint(3);
            _chillSprint = new ChillSprint(10);

            _allTaskView.OnTaskClicked += AddTask;
            _allTaskView.SetDevTasks(taskLibrary.GetAlLDevTasks());
            _allTaskView.OnCloseButtonClicked += RemoveSprint;
            _allTaskView.OnApplyButtonClicked += AllDevTaskApplyButtonClickListener;
            
            _localEvents.OnAllTaskShow += OpenAllTasks;
            //_localEvents.OnSprintContinue += RestoreSprint;
            _localEvents.OnDevActiveState += DevActiveStateListener;
            _localEvents.OnActiveSprintByType += ActiveSprintListener;
            _localEvents.OnWalkToIO += HideSprint;
            _localEvents.OnAutoSprintCreated += CreateAutoSprint;
            _localEvents.OnSprintExit += HideSprint;
        }

        private void ActiveSprintListener(SprintType type)
        {
            ApplyProgressToSprintTasks(9.5f, 2.5f);
        }

        private void CreateAutoSprint(SprintType sprintType)
        {
            HideSprint();
            
            if (sprintType == SprintType.Chill)
            {
                _currentSprint = _chillSprint;

                for (int i = 0; i < _currentSprint.Capacity; i++)
                {
                    ITask clone = _taskLibrary.GetChillTask.Clone();
                    clone.Progress = 100f;
                    
                    if (_currentSprint.TryAddTask(clone))
                    {
                        _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
                    }
                }
                
                _localEvents.TriggerSprintCreated(sprintType);
                //_localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type); //эта штука убирвет команду из панели
            }
        }

        private void UpdateActiveTasks()
        {
            if(_activeTasks.Count <= 0) return;

            foreach (var task in _activeTasks)
            {
                ApplyProgressToSprintTasks(9.5f, 0.7f);
            }
        }

        private void UpdateTasksQueue()
        {
            
        }

        private void DevActiveStateListener()
        {
            ApplyProgressToSprintTasks(9.5f, 0.7f);
        }

        private void AllDevTaskApplyButtonClickListener()
        {
            _localEvents.TriggerSprintCreated(SprintType.Dev);
        }

        private void OpenAllTasks()
        {
            _allTaskView.ShowAllTasks();
        }

        private void AddTask(ITask task)
        {
            _currentSprint = _devSprint;
        
            ITask clone = task.Clone();
            
            if (_currentSprint.TryAddTask(clone))
            {
                _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
            }
            
            _localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type);
        }

        private void ApplyProgressToSprintTasks(float delta, float interval)
        {
            if (_currentSprint == null || !_currentSprint.IsActiveSprint) 
                return;

            foreach (var task in _currentSprint.GetTasks())
            {
                if (!_pendingTasks.Contains(task) && !_activeTasks.Contains(task))
                    _pendingTasks.Enqueue(task);
            }

            _activeTasks.RemoveAll(task => task.IsCompleted);

            while (_activeTasks.Count < _maxSimultaneousUpdates && _pendingTasks.Count > 0)
            {
                var task = _pendingTasks.Dequeue();
                if (!task.IsCompleted)
                {
                    _activeTasks.Add(task);
                }
            }

            foreach (var task in _activeTasks)
            {
                Debug.Log($"Applying task to {task}. Progress: {task.Progress}");
                task.ApplyProgress(delta, interval);
            }

            if (_activeTasks.Count == 0 && _pendingTasks.Count == 0)
            {
                _localEvents.TriggerSprintClosed(_currentSprint.Type);
                RemoveSprint();
            }
        }

        private bool CheckCurrentDevSprint()
        {
            if (_devSprint.Tasks.Count <= 0)
            {
                return false;
            }

            return true;
        }

        // private void RemoveSprint()
        // {
        //     _sprintView.ClearTasks();
        //
        //     if (_currentSprint != null)
        //     {
        //         _currentSprint.ClearSprint();
        //         _localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type);
        //     }
        // }
        
        private void HideSprint(SprintType type)
        {
            if (_currentSprint == null) return;
            
            if(type == _currentSprint.Type) return;
                
            _hiddenTasks.Clear();
            _hiddenTasks.AddRange(_currentSprint.GetTasks());

            _sprintView.ClearTasks();
        
            _activeTasks.Clear();
            _pendingTasks.Clear();
            
            if (_currentSprint.Type == SprintType.Chill)
            {
                Debug.Log("Sprint Cleared");
                _currentSprint.ClearSprint();
                _localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type);
            }
        }
        
        private void HideSprint()
        {
            if (_currentSprint == null) return;

            _hiddenTasks.Clear();
            _hiddenTasks.AddRange(_currentSprint.GetTasks());

            _sprintView.ClearTasks();

            _activeTasks.Clear();
            _pendingTasks.Clear();
            
            if (_currentSprint.Type == SprintType.Chill)
            {
                Debug.Log("Sprint Cleared");
                _currentSprint.ClearSprint();
                _localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type);
            }
        }

        // Новый метод для восстановления спринта
        private void RestoreSprint(SprintType sprintType)
        {
            Debug.Log($"Restoring {sprintType}. Hidden tasks: {_hiddenTasks.Count}");

            // Устанавливаем текущий спринт
            _currentSprint = sprintType switch
            {
                SprintType.Dev => _devSprint,
                SprintType.Chill => _chillSprint,
                _ => _currentSprint
            };

            if (_currentSprint == null) return;

            // Восстанавливаем задачи в оригинальном порядке
            foreach (var task in _hiddenTasks.OrderBy(t => t.Progress))
            {
                var clone = task;
                if (_currentSprint.TryAddTask(clone))
                {
                    var taskView = _uiFactory.GetTaskView(_sprintView.ToDoField.transform);
                    _sprintView.AddTask(clone, taskView);
            
                    // Сразу добавляем в очередь обработки
                    _pendingTasks.Enqueue(clone); 
                    Debug.Log($"Restored task {clone.Id} (Progress: {clone.Progress})");
                }
            }

            // Запускаем обработку
            StartTaskProcessing();
    
            _isSprintHidden = false;
            _hiddenTasks.Clear();
        }

        private void StartTaskProcessing()
        {
            // Очищаем активные задачи
            _activeTasks.Clear();

            // Заполняем активные задачи с учетом лимита
            while (_activeTasks.Count < _maxSimultaneousUpdates && _pendingTasks.Count > 0)
            {
                var task = _pendingTasks.Dequeue();
                if (!task.IsCompleted)
                {
                    _activeTasks.Add(task);
                    Debug.Log($"Activated task: {task.Id}");
                }
            }
        }

        private void RemoveSprint()
        {
            _sprintView.ClearTasks();
            _hiddenTasks.Clear(); 

            if (_currentSprint != null)
            {
                Debug.Log("Sprint Cleared");
                _currentSprint.ClearSprint();
                _localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type);
            }
        }

        public void CleanUp()
        {
            _allTaskView.OnTaskClicked -= AddTask;
            _localEvents.OnAllTaskShow -= OpenAllTasks;
            _allTaskView.OnCloseButtonClicked -= RemoveSprint;
            _localEvents.OnDevActiveState -= DevActiveStateListener;
            _localEvents.OnWalkToIO -= HideSprint;
            _localEvents.OnAutoSprintCreated -= CreateAutoSprint;
            _localEvents.OnActiveSprintByType -= ActiveSprintListener;
            _localEvents.OnSprintExit -= HideSprint;
            //_localEvents.OnSprintContinue -= RestoreSprint;
        }
    }
}