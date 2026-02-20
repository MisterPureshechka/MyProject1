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
using Object = UnityEngine.Object;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp
    {
        private readonly SprintUI _sprintUI;
        private readonly LocalEvents _localEvents;
        private readonly SprintView _sprintView;
        private readonly TimeService _timeService;

        private readonly GameMetaConfig _gameMetaConfig;
        private readonly MilestoneRulesConfig _milestoneRulesConfig;
        private readonly Company _company;
        private readonly SaveService _saveService;

        private readonly List<IDevTask> _pendingTasks = new();
        private readonly List<IDevTask> _activeTasks = new();
        
        private readonly Dictionary<string, IDevTask> _assigned = new();
        
        private ProgressDataAdapter _progressDataAdapter;
        private ISprint _currentSprint;
        private UiFactory _uiFactory;
        private readonly GameStateMachine _gameStateMachine;

        private bool _sprintCompleted;
        private ProjectProgressService _projectProgressService;
        private EconomyService _economyService;
        private int _milestoneReward;

        public SprintSystem(SprintView sprintView, TimeService timeService, SprintUI sprintUI, LocalEvents localEvents, ProgressDataAdapter progressDataAdapter, UiFactory uiFactory, GameStateMachine gameStateMachine, GameMetaConfig gameStatMetaConfig, MilestoneRulesConfig milestoneRulesConfig, Company company, SaveService saveService, ProjectProgressService projectProgressService, EconomyService economyService)
        {
            _sprintView = sprintView;
            _timeService = timeService;
            _sprintUI = sprintUI;
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _uiFactory = uiFactory;
            _gameStateMachine = gameStateMachine;
            _gameMetaConfig = gameStatMetaConfig;
            _milestoneRulesConfig = milestoneRulesConfig;
            _company = company;
            _saveService = saveService;
            _projectProgressService = projectProgressService;
            _economyService = economyService;

            _sprintView.Init(_localEvents);
            
            LoadSprint();

            _company.OnEmployeeTick += ApplyProgressToSprint;
            _localEvents.OnDayPassed += OnDayPassed;
        }
        
        private void OnDayPassed()
        {
            var mp = _progressDataAdapter.Data.MilestoneProgress;

            if (!mp.IsActive || mp.IsCompleted || mp.IsFailed)
                return;

            mp.DaysSpent++;

            _localEvents.TriggerMilestoneProgressChanged();

            if (mp.DaysSpent >= mp.DaysLimit)
            {
                mp.IsFailed = true;
                mp.IsActive = false;
                _localEvents.TriggerMilestoneProgressChanged();
            }
        }
        
        private void StartMilestoneRun(MilestoneRunData milestone)
        {
            var mp = _progressDataAdapter.Data.MilestoneProgress;

            mp.IsActive = true;
            mp.MilestoneIndex = milestone.MilestoneIndex;
            mp.DaysLimit = milestone.DaysLimit;
            mp.DaysSpent = 0;

            mp.TotalTasks = milestone.Tasks.Count;
            mp.DoneTasks = 0;

            mp.IsCompleted = false;
            mp.IsFailed = false;

            _progressDataAdapter.Data.LastMilestoneResult.HasValue = false;

            _localEvents.TriggerMilestoneProgressChanged();
        }
        
        private async void LoadSprint()
        {
            int currentGameIndex = _progressDataAdapter.Data.GameIndex;
            int milestoneIndex = _progressDataAdapter.Data.CurrentMilestoneIndex;

            var milestone = MilestoneGenerator.Generate(
                currentGameIndex,
                milestoneIndex,
                _progressDataAdapter.Data.Stage, 
                _gameMetaConfig,
                _milestoneRulesConfig,
                _company.Employees
            );

            _milestoneReward = milestone.MoneyReward;
            
            StartMilestoneRun(milestone);
            
            _currentSprint = new DevSprint(milestone.Tasks.Count, null);

            for (int i = 0; i < milestone.Tasks.Count; i++)
                await AddTask(milestone.Tasks[i]);
            
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
                var progressData = _progressDataAdapter.Data;
                progressData.MilestoneProgress.DoneTasks++;
                progressData.Experience++;
                CompleteTask(progressData, task.Type);
                _saveService.SaveProgress(progressData);
                _localEvents.TriggerTaskComplete();
                _localEvents.TriggerMilestoneProgressChanged();

                _assigned.Remove(employee.Id);

                var next = TryAssignNextTask(employee);
                if (next == null)
                    employee.PauseWork();

                TryCompleteSprint();
            }
        }
        
        private void CompleteTask(ProgressData data, DevTaskType taskType)
        {
            if (data.MilestoneProgress == null)
                data.MilestoneProgress = new MilestoneProgressData();

            if (data.MilestoneProgress.DoneTasksByType == null)
                data.MilestoneProgress.DoneTasksByType = new Dictionary<DevTaskType, int>();

            // Добавление или обновление задач по типу
            if (data.MilestoneProgress.DoneTasksByType.ContainsKey(taskType))
                data.MilestoneProgress.DoneTasksByType[taskType]++;
            else
                data.MilestoneProgress.DoneTasksByType[taskType] = 1;

            // Увеличиваем общий прогресс задач
            data.MilestoneProgress.DoneTasks++;
            
            Debug.Log($"Task completed. Type: {taskType}, Count: {data.MilestoneProgress.DoneTasksByType[taskType]}");
        }

        
        private void TryCompleteSprint()
        {
            if (_sprintCompleted)
                return;

            if (_pendingTasks.Count > 0) return;
            if (_assigned.Count > 0) return;

            var all = _currentSprint.GetTasks();
            for (int i = 0; i < all.Count; i++)
                if (!all[i].IsCompleted)
                    return;

            _sprintCompleted = true;

            var mp = _progressDataAdapter.Data.MilestoneProgress;
            mp.IsActive = false;
            
            _economyService.ProcessMilestoneResult(_milestoneReward);

            bool releasedNow = _projectProgressService.OnMilestoneCompleted();

            _progressDataAdapter.Data.PendingReleaseWindow = releasedNow;

            _localEvents.TriggerMilestoneProgressChanged();

            _localEvents.TriggerMilestoneResultWindow();
        }

        
        private IDevTask TryAssignNextTask(Employee employee)
        {
            if (_pendingTasks.Count == 0)
                return null;

            // Найти задачу, которую сотрудник может выполнять
            for (int i = _pendingTasks.Count - 1; i >= 0; i--)
            {
                var candidate = _pendingTasks[i];
                if (employee.GetSkill(candidate.Type) >= 1)
                {
                    _pendingTasks.RemoveAt(i);
                    _assigned[employee.Id] = candidate;
                    employee.ResumeWork();
                    return candidate;
                }
            }

            // Нет задач под его скиллы
            return null;
        }

        private void CheckAllWorkCompleted()
        {
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
            _localEvents.OnDayPassed -= OnDayPassed;
            Object.Destroy(_sprintView.gameObject);
        }
    }
}