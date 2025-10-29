using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Scripts.Data;
using Scripts.GameDev;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Passion;
using Scripts.Perks;
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
        private readonly PerkService _perkService;
        private readonly TaskLibrary _taskLibrary;
        private readonly DevTaskCatalogue _devTaskCatalogue;
        private readonly ReadTaskCatalogue _readTaskCatalogue;
        
        private readonly DevSprintSaveService _devSave;
        
        private TextMeshProUGUI _tempStat;
        
        private SprintType _currentSprintType;
        private ISprint _currentSprint => _sprints.ContainsKey(_currentSprintType) ? _sprints[_currentSprintType] : null;

        private bool _isActiveState;
        private float _interval;
        
        private GameDevProgress _gameDevProgress;
        private const string currentGameName = "New Game";


        public SprintSystem(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents, InteractiveObjectRegisterer interactiveObjectRegisterer, ProgressDataAdapter progressDataAdapter, PerkService perkService, GameDevProgress gameDevProgress)
        {
            _tempStat = canvas.transform.Find("TempStat").GetComponent<TextMeshProUGUI>();
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _interactiveObjectRegisterer = interactiveObjectRegisterer;
            _progressDataAdapter = progressDataAdapter;
            _perkService = perkService;
            _taskLibrary = taskLibrary;

            _gameDevProgress = gameDevProgress;
            _gameDevProgress.CreateOrSelectGame(currentGameName);
            
            _devSave = new DevSprintSaveService(_progressDataAdapter, _taskLibrary);
            
            var devRestored = _devSave.Load();
            if (devRestored.Count > 0)
                _savedTasks[SprintType.Dev] = devRestored;

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
            _localEvents.OnHeroGetSprint += StartOrCreateSprint;
            _localEvents.OnHeroWalkToSprint += ExitSprint;
            _localEvents.OnHeroWalkToIO += ExitSprint;
            _localEvents.OnHeroGetRootIO += HeroGetRootIOListener;

            _sprints[SprintType.Dev] = new DevSprint(24, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Pc));
            _sprints[SprintType.Chill] = new ChillSprint(1, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Sofa));
            _sprints[SprintType.Eat] = new EatSprint(1, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Fridge));
            _sprints[SprintType.Read] = new ReadSprint(10, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Books));
            _sprints[SprintType.Play] = new PlaySprint(1, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.TV));
            _sprints[SprintType.Toilet] = new ToiletSprint(1, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Toilet));
            _sprints[SprintType.Shower] = new BathSprint(1, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Bath));
            _sprints[SprintType.CleanPc] = new CleanSprint(1, _interactiveObjectRegisterer.GetIOByType(InteractiveObjectType.Pc));
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

            var proto = _taskLibrary.GetAutoTasks(type);
            if (proto == null)
            {
                Debug.LogError($"[SprintSystem] No auto task set for {type}. Did you add it to TaskLibrary?");
                _localEvents.TriggerSprintCreated(type); // чтобы цепочка не зависла
                return;
            }

            for (int i = 0; i < _currentSprint.Capacity; i++)
            {
                var clone = proto.Clone();

                if (_currentSprint.TryAddTask(clone))
                {
                    Debug.Log($"[SprintSystem] Adding task '{clone.Title}' to view for {type}");
                    await _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform), type);
                    _pendingTasks.Add(clone);
                }
                else
                {
                    Debug.LogWarning($"[SprintSystem] TryAddTask returned false for {type}");
                }
            }

            _localEvents.TriggerSprintCreated(type);
        }

        private async void StartOrCreateSprint(SprintType sprintType)
        {
            while (_sprintView.IsBuisy)
                await Task.Yield();
            
            _currentSprintType = sprintType;
            
            _perkService.OnSprintStart(_currentSprintType);
            
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
                    _localEvents.TriggerShowCatalogue(_readTaskCatalogue);
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
            int baseMaxActive = 1; 
            int maxActiveTasks = _perkService.ModifyMaxActiveTasks(_currentSprint.Type, baseMaxActive);


            while (_activeTasks.Count < maxActiveTasks && _pendingTasks.Count > 0)
            {
                var nextTask = _pendingTasks[^1];
                _pendingTasks.RemoveAt(_pendingTasks.Count - 1);
                _activeTasks.Add(nextTask);
            }
            
            float health = _progressDataAdapter.GetStats(MetaType.Health);
            float maxHealth = _progressDataAdapter.GetMaxStats(MetaType.Health);

            float healthPercent = Mathf.Clamp01(health / maxHealth);
                
            float minInterval = 0.2f;
            float maxInterval = 3f;
            
            _localEvents.TriggerActiveSprintByType(_currentSprint.Type);
            
            float interval = Mathf.Lerp(maxInterval, minInterval, healthPercent);
            interval = _perkService.ModifyInterval(_currentSprint.Type, interval);
            
            for (int i = _activeTasks.Count - 1; i >= 0; i--) 
            {
                var task = _activeTasks[i];
                
                float taskInterval = _perkService.ModifyTaskInterval(task, interval);
                task.ApplyProgress(taskInterval);
                
                if (task.Progress <= 0f)
                {
                    if (task is IDevTask dev)
                    {
                        _gameDevProgress.CompleteTask(currentGameName, dev);
                        _localEvents.TriggerDevTaskComplete(dev.Type);
                    }
                    CheckSprintCompletion();
                    _activeTasks.RemoveAt(i);
                    _localEvents.TriggerPassionIncrease(PassionIncreaseType.TaskComplete);
                    _perkService.OnTaskCompleted(_currentSprint.Type, task);
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
            _perkService.OnSprintEnd(_currentSprintType);
            _localEvents.TriggerPassionIncrease(PassionIncreaseType.SprintComplete);
            _isActiveState = false;
            _localEvents.TriggerSprintExit();
            _localEvents.TriggerSprintComplete(_currentSprint.Type);
            
            if (_currentSprintType == SprintType.Dev)
                _devSave.Clear();

            if (_currentSprintType == SprintType.CleanPc)
            {
                _localEvents.TriggerIODirty(InteractiveObjectType.Pc, false);
            }
            
            await Task.Delay(500);
            await _sprintView.ClearTasks();
            _pendingTasks.Clear();
            _activeTasks.Clear();
            _currentSprint.ClearSprint();
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
            
            if (_currentSprint.Type == SprintType.Dev && _currentSprint.ShouldPersistTasksOnExit)
            {
                var all = _currentSprint.GetTasks();
                _savedTasks[SprintType.Dev] = new List<ITask>(all);
                _devSave.Save(_savedTasks[SprintType.Dev]);   
            }
            
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
            _localEvents.OnHeroGetSprint -= StartOrCreateSprint;
            _localEvents.OnSprintClosed -= ExitSprint;
            _localEvents.OnHeroGetRootIO -= HeroGetRootIOListener;
            _localEvents.OnHeroWalkToIO -= ExitSprint;
        }

        public void Execute(float deltatime)
        {
            if(_isActiveState) ApplyProgressToCurrentTask();
        }
    }
}