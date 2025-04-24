using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class SprintSystemTest
    {
        private TaskSystemDemo _demo;
        private readonly Dictionary<SprintType, ISprint> _sprints = new();
        private readonly Dictionary<SprintType, List<ITask>> _savedTasks = new();
        private readonly List<ITask> _pendingDevTasks = new();
        
        private readonly List<ITask> _pendingTasks = new();
        private readonly List<ITask> _activeTasks = new();
        
        private readonly SprintView _sprintView;
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;
        private readonly TaskLibrary _taskLibrary;
        private readonly AllTaskView _allTaskView;
        
        private SprintType _currentSprintType;
        private ISprint _currentSprint => _sprints.ContainsKey(_currentSprintType) ? _sprints[_currentSprintType] : null;
        

        public SprintSystemTest(TaskSystemDemo demo, TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents)
        {
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _taskLibrary = taskLibrary;
            
            _allTaskView = _uiFactory.GetAllTaskView(canvas.transform);
            
            _demo = demo;

            _sprints[SprintType.Dev] = new DevSprint(3);
            _sprints[SprintType.Chill] = new ChillSprint(3);
            
            _demo.enterCurrentSprintButton.onClick.AddListener(EnterToCurrentSprint);
            
            _demo.addDevTaskButton.onClick.AddListener(AddDevTask);
            _demo.addChillTaskButton.onClick.AddListener(AddChillTask);
            _demo.exitCurrentSprintButton.onClick.AddListener(ExitSprint);
            _demo.restoreDevSprintTaskButton.onClick.AddListener(RestoreDevSprint);
        }

        private void RestoreDevSprint()
        {
            var tasks = _savedTasks[SprintType.Dev];
            if(tasks == null) return;
            
            _currentSprintType = SprintType.Dev;
            
            foreach (var task in tasks)
            {
                if (_currentSprint.TryAddTask(task))
                {
                    var view = _uiFactory.GetTaskView(_sprintView.ToDoField.transform);
                    _sprintView.AddTask(task, view);

                    _pendingDevTasks.Add(task);
                }
            }
            
            _savedTasks[SprintType.Dev].Clear();
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
                _pendingDevTasks.Clear(); // Подготовим для новых
            }

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    if (sprint.TryAddTask(task))
                    {
                        var view = _uiFactory.GetTaskView(_sprintView.ToDoField.transform);
                        _sprintView.AddTask(task, view);
                        _pendingTasks.Add(task);
                    }
                }
            }

            StartTaskProcessing();
        }

        private void AddDevTask()
        {
            // if (_currentSprintType != SprintType.Dev)
            // {
            //     ExitSprintAsync();
            // }
            
            _currentSprintType = SprintType.Dev;
            
            var tasks = _taskLibrary.GetAlLDevTasks();
            var task = tasks[DevTaskType.Programming][Random.Range(0, 1)];
            
            ITask clone = task.Clone();
                        
            if (_currentSprint.TryAddTask(clone))
            {
                var view = _uiFactory.GetTaskView(_sprintView.ToDoField.transform);
                _sprintView.AddTask(clone, view);

                _pendingDevTasks.Add(clone);
            }
        }

        private void AddChillTask()
        {
            // if (_currentSprintType != SprintType.Chill)
            // {
            //     ExitSprintAsync();
            // }
            
            _currentSprintType = SprintType.Chill;

            for (int i = 0; i < _currentSprint.Capacity; i++)
            {
                var clone = _taskLibrary.GetChillTask.Clone();
                
                if (_currentSprint.TryAddTask(clone))
                {
                    _sprintView.AddTask(clone, _uiFactory.GetTaskView(_sprintView.ToDoField.transform));
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

            if (_currentSprintType == SprintType.Dev)
            {
                _savedTasks[_currentSprintType] = new List<ITask>(_pendingDevTasks);
            }

            await _sprintView.ClearTasks();
            _currentSprint.ClearSprint();
            _pendingDevTasks.Clear();
        }
        
        
        private void StartTaskProcessing()
        {
            // можно реализовать, например, перенос задач из ToDo -> InProgress -> Done
        }
    }
}