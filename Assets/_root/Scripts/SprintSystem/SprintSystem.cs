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
        // private readonly Dictionary<SprintType, ISprint> _sprints = new();
        // private readonly Dictionary<SprintType, List<ITask>> _savedTasks = new();
        //
        // private readonly SprintView _sprintView;
        // private readonly UiFactory _uiFactory;
        // private readonly LocalEvents _localEvents;
        // private readonly TaskLibrary _taskLibrary;
        //
        // private readonly List<ITask> _pendingTasks = new();
        // private readonly List<ITask> _activeTasks = new();
        //
        // private SprintType _currentSprintType;
        // private readonly AllTaskView _allTaskView;
        // private ISprint _currentSprint => _sprints.ContainsKey(_currentSprintType) ? _sprints[_currentSprintType] : null;
        //
        // public SprintSystem(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents)
        // {
        //     _sprintView = sprintView;
        //     _uiFactory = uiFactory;
        //     _localEvents = localEvents;
        //     _taskLibrary = taskLibrary;
        //
        //     _sprints[SprintType.Dev] = new DevSprint(3);
        //     _sprints[SprintType.Chill] = new ChillSprint(3);
        //     
        //     _allTaskView = _uiFactory.GetAllTaskView(canvas.transform);
        //
        //     
        //     _allTaskView.OnTaskClicked += AddTask;
        //     _allTaskView.SetDevTasks(taskLibrary.GetAlLDevTasks());
        //     _allTaskView.OnCloseButtonClicked += ExitSprint;
        //     _allTaskView.OnApplyButtonClicked += AllDevTaskApplyButtonClickListener;
        //         
        //     //_localEvents.OnAllTaskShow += OpenAllTasks;
        //     //_localEvents.OnSprintContinue += RestoreSprint;
        //     _localEvents.OnDevActiveState += DevActiveStateListener;
        //     //_localEvents.OnActiveSprintByType += ActiveSprintListener;
        //     //_localEvents.OnWalkToIO += HideSprint;
        //     _localEvents.OnAutoSprintCreated += EnterSprint;
        //     _localEvents.OnSprintExit += ExitSprint;
        //
        // }
        //
        // private void DevActiveStateListener()
        // {
        //     //ApplyProgressToSprintTasks(9.5f, 0.7f);
        // }
        //
        // private void AllDevTaskApplyButtonClickListener()
        // {
        //     _localEvents.TriggerSprintCreated(SprintType.Dev);
        // }
        //
        //
        // private void AddTask(ITask task)
        // {
        //     //_currentSprint = _devSprint;
        //      
        //     ITask clone = task.Clone();
        //                 
        //     if (_currentSprint.TryAddTask(clone))
        //     {
        //         _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
        //     }
        //          
        //     //_localEvents.TriggerActiveState(_currentSprint.IsActiveSprint, _currentSprint.Type);
        // }       
        //
        // // private void ApplyProgressToSprintTasks(float delta, float interval)
        // // {
        // //     if (_currentSprint == null) 
        // //         return;
        // //
        // //     foreach (var task in _currentSprint.GetTasks())
        // //     {
        // //         // if (!_pendingTasks.Contains(task) && !_activeTasks.Contains(task))
        // //         //     _pendingTasks.Enqueue(task);
        // //     }
        // //
        // //     _activeTasks.RemoveAll(task => task.IsCompleted);
        // //
        // //     while (_activeTasks.Count < _maxSimultaneousUpdates && _pendingTasks.Count > 0)
        // //     {
        // //         var task = _pendingTasks.Dequeue();
        // //         if (!task.IsCompleted)
        // //         {
        // //             _activeTasks.Add(task);
        // //         }
        // //     }
        // //
        // //     foreach (var task in _activeTasks)
        // //     {
        // //         Debug.Log($"Applying task to {task}. Progress: {task.Progress}");
        // //         task.ApplyProgress(delta, interval);
        // //     }
        // //
        // //     if (_activeTasks.Count == 0 && _pendingTasks.Count == 0)
        // //     {
        // //         _localEvents.TriggerSprintClosed(_currentSprint.Type);
        // //         ExitSprint();
        // //     }
        // // }
        //
        //
        //
        // public void EnterSprint(SprintType type)
        // {
        //     ExitSprint();
        //     _currentSprintType = type;
        //
        //     var sprint = _currentSprint;
        //     if (sprint == null) return;
        //
        //     var tasks = _savedTasks.ContainsKey(type) ? _savedTasks[type] : CreateAutoTasksForSprint(type);
        //
        //     foreach (var task in tasks)
        //     {
        //         if (sprint.TryAddTask(task))
        //         {
        //             var view = _uiFactory.GetTaskView(_sprintView.ToDoField.transform);
        //             _sprintView.AddTask(task, view);
        //             _pendingTasks.Add(task);
        //         }
        //     }
        //
        //     StartTaskProcessing();
        // }
        //
        // public void ExitSprint()
        // {
        //     if (_currentSprint == null) return;
        //
        //     if (_currentSprint.ShouldPersistTasksOnExit)
        //     {
        //         _savedTasks[_currentSprint.Type] = _currentSprint.GetTasks();
        //     }
        //
        //     _sprintView.ClearTasks();
        //     _pendingTasks.Clear();
        //     _activeTasks.Clear();
        //     _currentSprint.ClearSprint();
        // }
        //
        // private List<ITask> CreateAutoTasksForSprint(SprintType type)
        // {
        //     var list = new List<ITask>();
        //     var sprint = _sprints[type];
        //     for (int i = 0; i < sprint.Capacity; i++)
        //     {
        //         //list.Add(_taskLibrary.GetChillTask.Clone());
        //     }
        //     return list;
        // }
        //
        // private void StartTaskProcessing()
        // {
        //     // можно реализовать, например, перенос задач из ToDo -> InProgress -> Done
        // }
        //
        public void CleanUp()
        {
            throw new NotImplementedException();
        }
    }
}