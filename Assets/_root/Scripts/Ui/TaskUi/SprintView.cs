using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class SprintView : MonoBehaviour
    {
        private List<TaskView> _taskViews = new();

        public void AddTask(ITask task, GameObject taskGO)
        {
            var taskView = Object.Instantiate(taskGO, gameObject.transform).GetComponent<TaskView>();
            taskView.SetInfo(task.Title, task.Progress);
            _taskViews.Add(taskView);
        }

        public async void ClearTasks()
        {
            var tasksToRemove = new List<TaskView>(_taskViews);
    
            foreach (var taskView in tasksToRemove)
            {
                await Task.Delay(200); 
        
                _taskViews.Remove(taskView); 
                Destroy(taskView.gameObject); 
            }

        }
        
    }
}