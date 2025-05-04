using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class SprintSystemTest : ICleanUp
    {
        private TaskSystemDemo _demo;
        private readonly Dictionary<SprintType, ISprint> _sprints = new();
        private readonly Dictionary<SprintType, List<ITask>> _savedTasks = new();
        
        private readonly List<ITask> _pendingTasks = new();
        private readonly List<ITask> _activeTasks = new();
        
        private readonly SprintView _sprintView;
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;
        private readonly TaskLibrary _taskLibrary;
        private readonly AllTaskView _allTaskView;
        
        private SprintType _currentSprintType;
        private ISprint _currentSprint => _sprints.ContainsKey(_currentSprintType) ? _sprints[_currentSprintType] : null;


        public SprintSystemTest(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents)
        {
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _taskLibrary = taskLibrary;
            
            _allTaskView = _uiFactory.GetAllTaskView(canvas.transform);
            _allTaskView.SetDevTasks(taskLibrary.GetAlLDevTasks());
            _allTaskView.OnCloseButtonClicked += ExitSprint;
            _allTaskView.OnApplyButtonClicked += AllDevTaskApplyButtonClickListener;
            
            _allTaskView.OnTaskClicked += AddTask;
            
            _localEvents.OnAllTaskShow += OpenAllTasks;
            _localEvents.OnSprintContinue += TryRestoreSprint;
            
            //_demo = demo;

            _sprints[SprintType.Dev] = new DevSprint(12);
            _sprints[SprintType.Chill] = new ChillSprint(3);
            _sprints[SprintType.Eat] = new EatSprint(5);


            // _demo.enterCurrentSprintButton.onClick.AddListener(EnterToCurrentSprint);
            //
            // _demo.addDevTaskButton.onClick.AddListener(AddDevTask);
            // _demo.addChillTaskButton.onClick.AddListener(AddChillTask);
            // _demo.exitCurrentSprintButton.onClick.AddListener(ExitSprint);
            // _demo.restoreDevSprintTaskButton.onClick.AddListener(RestoreDevSprint);
            // _demo.applyProgressToCurrentTaskButton.onClick.AddListener(ApplyProgressToCurrentTask);
            // _demo.addEatTaskButton.onClick.AddListener(AddEatTask);
            // _demo.restoreEatSprintTaskButton.onClick.AddListener(RestoreEatSprint);
        }

        private void OpenAllTasks()
        {
            _allTaskView.ShowAllTasks();
        }

        private async void TryRestoreSprint(SprintType type)
        {
            if (!_currentSprint.ShouldPersistTasksOnExit) return;
            
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _savedTasks.TryGetValue(type, out var tasks);

            if (tasks == null) return;

            _currentSprintType = type;

            _pendingTasks.Clear();
            _activeTasks.Clear();

            foreach (var task in tasks)
            {
                if (_currentSprint.TryAddTask(task))
                {
                    await _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));

                    _pendingTasks.Add(task);
                }
            }

            _savedTasks[type].Clear();

            ApplyProgressToCurrentTask();
            
        }
        
        private void AllDevTaskApplyButtonClickListener()
        {
            _localEvents.TriggerSprintCreated(SprintType.Dev);
        }

        private async void RestoreDevSprint()
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _savedTasks.TryGetValue(SprintType.Dev, out var tasks);

            if (tasks == null) return;

            _currentSprintType = SprintType.Dev;

            _pendingTasks.Clear();
            _activeTasks.Clear();

            foreach (var task in tasks)
            {
                if (_currentSprint.TryAddTask(task))
                {
                    await _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));

                    _pendingTasks.Add(task);
                }
            }

            _savedTasks[SprintType.Dev].Clear();

            ApplyProgressToCurrentTask();
        }

        private async void AddTask(ITask task)
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            ITask clone = task.Clone();
                        
            if (_currentSprint.TryAddTask(clone))
            {
                await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
            }   
        } 

        private async void RestoreEatSprint()
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _savedTasks.TryGetValue(SprintType.Eat, out var tasks);

            if (tasks == null) return; 

            _currentSprintType = SprintType.Eat;

            _pendingTasks.Clear();
            _activeTasks.Clear();

            foreach (var task in tasks)
            {
                if (_currentSprint.TryAddTask(task))
                {
                    await _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
                    
                    _pendingTasks.Add(task);
                }
            }

            _savedTasks[SprintType.Eat].Clear();
            
            ApplyProgressToCurrentTask();
        }

        private void ApplyProgressToCurrentTask()
        {
            const int MaxActiveTasks = 2; 

            while (_activeTasks.Count < MaxActiveTasks && _pendingTasks.Count > 0)
            {
                var nextTask = _pendingTasks[^1];
                _pendingTasks.RemoveAt(_pendingTasks.Count - 1);
                _activeTasks.Add(nextTask);
                Debug.Log($"[SprintSystemTest] Added to active: {nextTask}");
            }
            
            for (int i = _activeTasks.Count - 1; i >= 0; i--) 
            {
                var task = _activeTasks[i];
            
                task.ApplyProgress(15f, 0);
            
                if (task.Progress <= 0f)
                {
                    CheckSprintCompletion();
                    _activeTasks.RemoveAt(i);
                }
            }
        }

        private void CheckSprintCompletion()
        {
            if (_currentSprint == null) return;

            var tasks = _currentSprint.GetTasks();

            bool hasUnfinishedTask = false;

            foreach (var task in tasks)
            {
                if (!task.IsCompleted) 
                {
                    hasUnfinishedTask = true;
                    break;
                }
            }

            if (!hasUnfinishedTask)
            {
                CompleteSprint();
            }
        }

        private async void CompleteSprint()
        {
            await Task.Delay(500);
            await _sprintView.ClearTasks();
            _pendingTasks.Clear();
            _activeTasks.Clear();
            _currentSprint.ClearSprint();
        }


        public void UpdateStats()
        {
            string result = "";
            
            foreach (var key in _sprints.Keys)
            {
                result += $"{key}: {_sprints[key].GetTasks().Count}\n";
            }
            
            result += "\n";
            
            foreach (var key in _savedTasks.Keys)
            {
                result += $" saved tasks = {key}: {_savedTasks[key].Count}\n";
            }
            
            result += $"Active tasks: {_activeTasks.Count}\n";
            result += $"is SprintView buisy: {_sprintView.IsBuisy}\n";
            _demo._allSprintsText.text = result;
        }

        private async void EnterToCurrentSprint()
        {
            await EnterSprintAsync(_currentSprintType);
        }

        public async Task EnterSprintAsync(SprintType type)
        {
            await ExitSprintAsync();

            _currentSprintType = type;

            var sprint = _currentSprint;
            if (sprint == null) return;

            List<ITask> tasks = null;
            
            if (type == SprintType.Dev && _savedTasks.ContainsKey(type))
            {
                tasks = _savedTasks[type];
                _pendingTasks.Clear(); 
            }

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    if (sprint.TryAddTask(task))
                    {
                        await _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
                        _pendingTasks.Add(task);
                    }
                }
            }

            StartTaskProcessing();
        }

        private async void AddDevTask()
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _currentSprintType = SprintType.Dev;
            
            var tasks = _taskLibrary.GetAlLDevTasks();
            var task = tasks[DevTaskType.Programming][Random.Range(0, 1)];
            
            ITask clone = task.Clone();
                        
            if (_currentSprint.TryAddTask(clone))
            {
                await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));

                _pendingTasks.Add(clone);
            }
        }

        private async void AddEatTask()
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _currentSprintType = SprintType.Eat;
            
            var task = _taskLibrary.GetRandomEatTask();;
            
            ITask clone = task.Clone();
                        
            if (_currentSprint.TryAddTask(clone))
            {
                await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));

                _pendingTasks.Add(clone);
            }
        
        }

        private async void AddChillTask()
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _currentSprintType = SprintType.Chill;

            for (int i = 0; i < _currentSprint.Capacity; i++)
            {
                var clone = _taskLibrary.GetChillTask.Clone();
                
                if (_currentSprint.TryAddTask(clone))
                {
                    await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
                    
                    _pendingTasks.Add(clone);
                }
            }
        }

        private async void ExitSprint()
        {
            await ExitSprintAsync();
        }

        private async Task ExitSprintAsync()
        {
            if (_currentSprintType == null) return;

            if (_currentSprint.ShouldPersistTasksOnExit)
            {
                var allTasks = _currentSprint.GetTasks();
                _savedTasks[_currentSprintType] = new List<ITask>(allTasks);
            }

            await _sprintView.ClearTasks();
            _currentSprint.ClearSprint();
            _pendingTasks.Clear();
            _activeTasks.Clear();
        }
        
        
        private void StartTaskProcessing()
        {
            // можно реализовать, например, перенос задач из ToDo -> InProgress -> Done
        }

        public void CleanUp()
        {
            _allTaskView.OnCloseButtonClicked -= ExitSprint;
            _allTaskView.OnApplyButtonClicked -= AllDevTaskApplyButtonClickListener;
            _localEvents.OnSprintContinue -= TryRestoreSprint;
            _allTaskView.OnTaskClicked -= AddTask;
        }
    }
}