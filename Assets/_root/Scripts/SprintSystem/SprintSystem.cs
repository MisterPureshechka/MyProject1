using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp, IExecute
    {
        private TaskSystemDemo _demo;
        private readonly Dictionary<SprintType, ISprint> _sprints = new();
        private readonly Dictionary<SprintType, List<ITask>> _savedTasks = new();
        private readonly Dictionary<string, Dictionary<string, float>> _effects;
        
        private readonly List<ITask> _pendingTasks = new();
        private readonly List<ITask> _activeTasks = new();
        
        private readonly SprintView _sprintView;
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;
        private readonly InteractiveObjectRegisterer _interactiveObjectRegisterer;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly TaskLibrary _taskLibrary;
        private readonly DevTaskCatalogue _devTaskCatalogue;
        private readonly ReadTaskCatalogue _readTaskCatalogue;
        
        private TextMeshProUGUI _tempStat;
        
        private SprintType _currentSprintType;
        private ISprint _currentSprint => _sprints.ContainsKey(_currentSprintType) ? _sprints[_currentSprintType] : null;

        private bool _isActiveState;
        private float _interval;


        public SprintSystem(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents, InteractiveObjectRegisterer interactiveObjectRegisterer, ProgressDataAdapter progressDataAdapter)
        {
            _tempStat = canvas.transform.Find("TempStat").GetComponent<TextMeshProUGUI>();
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _interactiveObjectRegisterer = interactiveObjectRegisterer;
            _progressDataAdapter = progressDataAdapter;
            _taskLibrary = taskLibrary;

            _effects = StatEffectLoader.Load();
            
            _devTaskCatalogue = _uiFactory.GetAllTaskView(canvas.transform);
            _devTaskCatalogue.Init(_localEvents);
            _devTaskCatalogue.SetDevTasks(taskLibrary.GetAlLDevTasks());
            _devTaskCatalogue.OnCloseButtonClicked += CloseCatalogButtonClickedListener;
            _devTaskCatalogue.OnApplyButtonClicked += CatalogueApplyButtonClickListener;
            
            _readTaskCatalogue = _uiFactory.GetReadTaskCatalogue(canvas.transform);
            _readTaskCatalogue.Init(_localEvents);
            _readTaskCatalogue.SetReadTask(taskLibrary.GetReadTasks());
            _readTaskCatalogue.OnCloseButtonClicked += CloseCatalogButtonClickedListener;
            _readTaskCatalogue.OnApplyButtonClicked += CatalogueApplyButtonClickListener;
            
            _devTaskCatalogue.OnTaskClicked += AddTask;
            _readTaskCatalogue.OnTaskClicked += AddTask;
            
            _localEvents.OnSprintClosed += ExitSprint;
            _localEvents.OnHeroGetIO += StartOrCreateSprint;
            _localEvents.OnHeroWalkToIO += ExitSprint;
            _localEvents.OnHeroGetRootIO += HeroGetRootIOListener;

            _sprints[SprintType.Dev] = new DevSprint(12, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.Pc));
            _sprints[SprintType.Chill] = new ChillSprint(1, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.Chair));
            _sprints[SprintType.Eat] = new EatSprint(1, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.Fridge));
            _sprints[SprintType.Read] = new ReadSprint(10, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.Books));
            _sprints[SprintType.Play] = new PlaySprint(1, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.TV));
            _sprints[SprintType.Toilet] = new ToiletSprint(1, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.Toilet));
            _sprints[SprintType.Shower] = new BathSprint(1, _interactiveObjectRegisterer.GetRootByIOType(InteractiveObjectType.Bath)); 
        }

        private void HeroGetRootIOListener(SprintType type)
        {
            _isActiveState = true;
        }

        private void CloseCatalogButtonClickedListener()
        {
            _localEvents.TriggerTaskCatalogHide(_currentSprintType);
            ExitSprint();
        }

        private async void CreateAutoSprint(SprintType type)
        {
            _currentSprintType = type;
            
            for (int i = 0; i < _currentSprint.Capacity; i++)
            {
                var clone = _taskLibrary.GetAutoTasks(type).Clone();
                
                if (_currentSprint.TryAddTask(clone))
                {
                    await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform), type);
                    
                    _pendingTasks.Add(clone);
                }
            }
            
            _localEvents.TriggerSprintCreated(type);
        }

        private async void StartOrCreateSprint(SprintType sprintType)
        {
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
                    ShowCatalogue(sprintType);
                }
                else
                {
                    CreateAutoSprint(_currentSprint.Type);
                }
            }
        }

        private void ShowCatalogue(SprintType sprintType)
        {
            _isActiveState = false;
            
            switch (sprintType)
            {
                case SprintType.Dev:
                    _localEvents.TriggerShowCatalogue(_devTaskCatalogue);
                    break;
                case SprintType.Read:
                    _readTaskCatalogue.ShowCatalogue();
                    break;
                default:
                    Debug.LogError($"{nameof(SprintType)} doesn't have catalogue");
                    return;
            }
        }

        private async Task TryRestoreSprint(SprintType type)
        {
            if (!_currentSprint.ShouldPersistTasksOnExit) return;
            
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _savedTasks.TryGetValue(type, out var tasks);

            if (tasks == null || tasks.Count == 0) return;

            _currentSprintType = type;

            _pendingTasks.Clear();
            _activeTasks.Clear();

            foreach (var task in tasks)
            {
                if (_currentSprint.TryAddTask(task))
                {
                    await _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.ToDoField.transform), type);

                    _pendingTasks.Add(task);
                }
            }

            _savedTasks[type].Clear();
            
            ApplyProgressToCurrentTask();
            
            _localEvents.TriggerSprintCreated(_currentSprintType);
        }
        
        
        
        private void CatalogueApplyButtonClickListener()
        {
            _localEvents.TriggerSprintCreated(_currentSprintType);
        }

        private async void AddTask(ITask task)
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            ITask clone = task.Clone();
                        
            if (_currentSprint.TryAddTask(clone))
            {
                await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform), _currentSprintType);
                _pendingTasks.Add(clone);
            }   
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
            
            float health = _progressDataAdapter.GetStats(MetaType.Health);
            float maxHealth = _progressDataAdapter.GetMaxStats(MetaType.Health);

            float healthPercent = Mathf.Clamp01(health / maxHealth);
                
            float minInterval = 0.2f;
            float maxInterval = 3f;
            
            _localEvents.TriggerActiveSprintByType(_currentSprint.Type);
            
            float interval = Mathf.Lerp(maxInterval, minInterval, healthPercent);
            
            for (int i = _activeTasks.Count - 1; i >= 0; i--) 
            {
                var task = _activeTasks[i];
                
                task.ApplyProgress(interval);
                
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
            _localEvents.TriggerSprintExit();
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
            result += $"Health = {_progressDataAdapter.GetStats(MetaType.Health)}\n";
            _tempStat.text = result;
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
            _localEvents.TriggerSprintExit();
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
            _devTaskCatalogue.OnCloseButtonClicked -= ExitSprint;
            _devTaskCatalogue.OnApplyButtonClicked -= CatalogueApplyButtonClickListener;
            _devTaskCatalogue.OnTaskClicked -= AddTask;
            _readTaskCatalogue.OnTaskClicked -= AddTask;
            _readTaskCatalogue.OnCloseButtonClicked -= CloseCatalogButtonClickedListener;
            _readTaskCatalogue.OnApplyButtonClicked -= CatalogueApplyButtonClickListener;
            _localEvents.OnHeroGetIO -= StartOrCreateSprint;
            _localEvents.OnSprintClosed -= ExitSprint;
            _localEvents.OnHeroGetRootIO -= HeroGetRootIOListener;
        }

        public void Execute(float deltatime)
        {
            if(_isActiveState) ApplyProgressToCurrentTask();
            UpdateStats();
        }
    }
}