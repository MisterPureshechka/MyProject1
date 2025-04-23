using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class SprintView : MonoBehaviour
    {
        private List<TaskView> _taskViews = new();
        private Dictionary<string, TaskView> _taskIdToViewMap = new();

        [field: SerializeField] public Transform ToDoField;
        [SerializeField] private Transform _inProgress;
        [SerializeField] private Transform _done;

        public void AddTask(ITask task, TaskView taskView)
        {
            string uniqueKey = $"{task.Id}_{Guid.NewGuid()}";

            if (task is IDevTask devTask)
            {
                taskView.SetInfoIfDev(task.Title, task.Progress, devTask.Type);
            }
            else
            {
                taskView.SetInfo(task.Title, task.Progress);
            }
            
            _taskViews.Add(taskView);
            _taskIdToViewMap[uniqueKey] = taskView;  
        
            task.OnProgressChanged += (changedTask, value) => OnTaskProgressChanged(uniqueKey, changedTask, value);
            task.OnTaskCompleted += (completedTask) => OnTaskCompleted(uniqueKey, completedTask);
        }

        private void OnTaskProgressChanged(string uniqueKey, ITask task, float value)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                if (taskView != null)
                {
                    taskView.transform.SetParent(_inProgress);
                    taskView.UpdateProgress(task.Progress, value);
                }
            }
        }

        private void OnTaskCompleted(string uniqueKey, ITask task)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                taskView.transform.SetParent(_done);
            }
        }

        public async void ClearTasks()
        {
            foreach (var taskView in _taskViews)
            {
                //await Task.Delay(200);
                Destroy(taskView.gameObject);
            }
        
            _taskViews.Clear();
            _taskIdToViewMap.Clear();
        }
    }
}