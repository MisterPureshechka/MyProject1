using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class SprintSystemTest : ICleanUp, IExecute
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
        
        private TextMeshProUGUI _tempStat;
        
        private SprintType _currentSprintType;
        private ISprint _currentSprint => _sprints.ContainsKey(_currentSprintType) ? _sprints[_currentSprintType] : null;

        private bool _isActiveState;


        public SprintSystemTest(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents, InteractiveObjectRegisterer interactiveObjectRegisterer)
        {
            _tempStat = canvas.transform.Find("TempStat").GetComponent<TextMeshProUGUI>();
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _taskLibrary = taskLibrary;
            
            _allTaskView = _uiFactory.GetAllTaskView(canvas.transform);
            _allTaskView.Init(_localEvents);
            _allTaskView.SetDevTasks(taskLibrary.GetAlLDevTasks());
            _allTaskView.OnCloseButtonClicked += CloseCatalogButtonClickedListener;
            _allTaskView.OnApplyButtonClicked += AllDevTaskApplyButtonClickListener;
            
            _allTaskView.OnTaskClicked += AddTask;
            
            _localEvents.OnTaskCatalogShow += OpenAllTasks;
            //_localEvents.OnSprintContinue += TryRestoreSprint;
            _localEvents.OnSprintClosed += ExitSprint;
            _localEvents.OnHeroGetIO += StartOrCreateSprint;
            _localEvents.OnHeroWalkToIO += ExitSprint;
            
            //_demo = demo;

            //_sprints[SprintType.Dev] = new DevSprint(12, );
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

        private void CloseCatalogButtonClickedListener()
        {
            _localEvents.TriggerTaskCatalogHide(SprintType.Dev);
        }

        private async void CreateAutoSprint(SprintType type)
        {
            // if(_currentSprint != null)
            //     await ExitSprintAsync();
            
            // while (_sprintView.IsBuisy)
            //     await Task.Yield();
            //
            _currentSprintType = type;
            
            for (int i = 0; i < _currentSprint.Capacity; i++)
            {
                var clone = _taskLibrary.GetAutoTasks(type).Clone();
                
                if (_currentSprint.TryAddTask(clone))
                {
                    await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
                    
                    _pendingTasks.Add(clone);
                }
            }
            
            _isActiveState = true;
            _localEvents.TriggerSprintCreated(type);
        }

        private async void StartOrCreateSprint(SprintType sprintType)
        {
            // if(_currentSprint != null)
            //     await ExitSprintAsync();
            
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _currentSprintType = sprintType;
            
            if (_currentSprint.ShouldPersistTasksOnExit)
            {
                await TryRestoreSprint(sprintType); 
            }

            if (_currentSprint.GetTasks().Count <= 0)
            {
                if (_currentSprint.HasCatalog)
                {
                    _allTaskView.ShowAllTasks();  
                }
                else
                {
                    CreateAutoSprint(_currentSprint.Type);
                }
            }
            else
            {
                _isActiveState = true;
            }
            
            _localEvents.TriggerSprintCreated(sprintType);
        }

        private void OpenAllTasks(SprintType sprintType)
        {
            _isActiveState = false;
            _allTaskView.ShowAllTasks();
            _currentSprintType = sprintType;
        }

        private async Task TryRestoreSprint(SprintType type)
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
            _isActiveState = true;
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
                _pendingTasks.Add(clone);
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
            
                task.ApplyProgress(25.1f, 1);
                
                _localEvents.TriggerActiveSprintByType(_currentSprint.Type);
                
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
            _isActiveState = false;
            _localEvents.TriggerSprintComplete(_currentSprint.Type);
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
            result += $"_isActiveState = {_isActiveState}\n";
            _tempStat.text = result;
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

        private async void ExitSprint()
        {
            await ExitSprintAsync();
        }
        
        private async void ExitSprint(SprintType type)
        {
            await ExitSprintAsync();
        }

        private async Task ExitSprintAsync()
        {
            _isActiveState = false;
            if (_currentSprint == null) return;
            
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

        public void CleanUp()
        {
            _allTaskView.OnCloseButtonClicked -= ExitSprint;
            _allTaskView.OnApplyButtonClicked -= AllDevTaskApplyButtonClickListener;
            //_localEvents.OnSprintContinue -= TryRestoreSprint;
            _allTaskView.OnTaskClicked -= AddTask;
            _localEvents.OnTaskCatalogShow -= OpenAllTasks;
            _localEvents.OnAutoSprintCreated -= CreateAutoSprint;
            _localEvents.OnHeroGetIO -= StartOrCreateSprint;
            _localEvents.OnSprintClosed -= ExitSprint;
        }

        public void Execute(float deltatime)
        {
            if(_isActiveState) ApplyProgressToCurrentTask();
            UpdateStats();
        }
    }
}