using Core;
using Scripts.Data;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Tasks
{
    public class SprintSystem : ICleanUp
    {
        private readonly GameData _gameData;

        private readonly TaskPanelView _taskPanelView;
        private readonly SprintView _sprintView;
        private readonly UiFactory _uiFactory;

        private DevSprint _devSprint;
        private EatSprint _eatSprint;
    
        private SprintBase _currentSprint;
        
        public SprintSystem(TaskLibrary taskLibrary, Canvas canvas, GameData gameData, SprintView sprintView, UiFactory uiFactory)
        {
            _gameData = gameData;
            _sprintView = sprintView;
            _uiFactory = uiFactory;
            _taskPanelView = _uiFactory.GetTaskPanelView(canvas.transform);

            _devSprint = new DevSprint(4);
            _eatSprint = new EatSprint(3);
            
            
            _taskPanelView.SetTaskLibrary(taskLibrary);
            //_taskPanelView.ShowTasksForSprint(SprintType.Dev);
            _taskPanelView.OnTaskClicked += AddTask;
        }
        
        private void OpenDevTaskView()
        {
            _currentSprint = _devSprint;
        
            var sprint = _currentSprint;
        
            var tasks = sprint.GetTasks();
        
            for (int i = 0; i < tasks.Count; i++)
            {
                _sprintView.AddTask(tasks[i], _uiFactory.GetTaskView(_sprintView.transform));
            }
        }
    
        private void OpenEatTaskView()
        {
            _currentSprint = _eatSprint;
        
            var sprint = _currentSprint;
        
            var tasks = sprint.GetTasks();
        
            for(int i = 0; i < tasks.Count; i++)
            {
                _sprintView.AddTask(tasks[i], _uiFactory.GetTaskView(_sprintView.transform));
            }
        }

        private void AddTask(ITask task)
        {
            _currentSprint = _devSprint;
        
            if (_currentSprint.TryAddTask(task))
            {
                Debug.Log($"Add task: {task}");
                _sprintView.AddTask(task, _uiFactory.GetTaskView(_sprintView.transform));
            }
        }

        private void HideSprint()
        {
            _sprintView.ClearTasks();
        }

        public void CleanUp()
        {
            _taskPanelView.OnTaskClicked -= AddTask;
        }
    }
}