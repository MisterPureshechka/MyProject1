using Scripts.Tasks;
using Scripts.Ui.TaskUi;
using UnityEngine;
using UnityEngine.UI;

public class TaskSystemDemo : MonoBehaviour
{
    private TaskLibrary _taskLibrary;
    [SerializeField] private TaskPanelView _taskPanelView;
    
    [SerializeField] private SprintView _sprintView;
    [SerializeField] private GameObject _taskView;
    
    
    private DevSprint _devSprint;
    private EatSprint _eatSprint;
    
    private DevTask _devTask;
    private EatTask _eatTask;
    
    private SprintBase _currentSprint;

    private void Start()
    {
        _taskLibrary = new TaskLibrary();
        _taskPanelView.SetTaskLibrary(_taskLibrary);
        _taskPanelView.ShowTasksForSprint(SprintType.Dev);
        _taskPanelView.OnTaskClicked += AddTask;
        
        
        _devSprint = new DevSprint(4);
        _eatSprint = new EatSprint(3);
        
        _devTask = new DevTask(DevTaskType.Programming, "Programming", 100f);
        _eatTask = new EatTask(EatTaskType.coffee, "Coffee", 100f);
        
        
    }

    private void OpenDevTaskView()
    {
        _currentSprint = _devSprint;
        
        var sprint = _currentSprint;
        
        var tasks = sprint.GetTasks();
        
        for (int i = 0; i < tasks.Count; i++)
        {
            _sprintView.AddTask(tasks[i], _taskView);
        }
    }
    
    private void OpenEatTaskView()
    {
        _currentSprint = _eatSprint;
        
        var sprint = _currentSprint;
        
        var tasks = sprint.GetTasks();
        
        for (int i = 0; i < tasks.Count; i++)
        {
            _sprintView.AddTask(tasks[i], _taskView);
        }
    }

    private void AddTask(ITask task)
    {
        _currentSprint = _devSprint;
        
        if (_currentSprint.TryAddTask(task))
        {
            _sprintView.AddTask(task, _taskView);
        }
    }

    private void HideSprint()
    {
        _sprintView.ClearTasks();
    }

    private void OnDestroy()
    {
        _taskPanelView.OnTaskClicked -= AddTask;
    }
}
