using System;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Hero;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp
    {
        public Action OnPanelClosed;
        private readonly GameData _gameData;

        private readonly AllTaskView _allTaskView;
        private readonly SprintView _sprintView;
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;

        private DevSprint _devSprint;
        private EatSprint _eatSprint;
    
        private SprintBase _currentSprint;
        
        private Vector3 _panelPosition;
        
        public SprintSystem(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory, LocalEvents localEvents)
        {
            _gameData = gameData;
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _allTaskView = _uiFactory.GetAllTaskView(canvas.transform);

            _devSprint = new DevSprint(4);
            _eatSprint = new EatSprint(3);

            _allTaskView.OnTaskClicked += AddTask;
            _allTaskView.SetDevTasks(taskLibrary.GetAlLDevTasks());
            _localEvents.OnGetHeroPos += GetPanelPosition;
        }

        private void OpenAllTasks(SprintType type)
        {
            _allTaskView.ShowAllTasks();
        }

        private void GetPanelPosition(Vector3 position)
        {
            _panelPosition = position;
        }

        private void AddTask(ITask task)
        {
            _currentSprint = _devSprint;
        
            if (_currentSprint.TryAddTask(task))
            {
                _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.transform));
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

        private void HideSprint()
        {
            _sprintView.ClearTasks();
        }

        public void CleanUp()
        {
            _allTaskView.OnTaskClicked -= AddTask;
            //_taskPanelView.OnTaskClicked -= AddTask;
            _localEvents.OnGetHeroPos -= GetPanelPosition;
        }
    }
}