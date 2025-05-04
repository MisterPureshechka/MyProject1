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
        private bool _isBuisy;

        public async Task AddTask(ITask task, TaskView taskView)
        {
            while (_isBuisy)
                await Task.Yield();
            
            string uniqueKey = $"{task.Id}_{Guid.NewGuid()}";

            if (task is IDevTask devTask)
            {
                taskView.SetInfoIfDev(task.Title, task.Progress, devTask.Type);
                taskView.ShowTask();
            }
            else
            {
                taskView.SetInfo(task.Title, task.Progress);
                taskView.ShowTask();
            }
            
            _taskViews.Add(taskView);
            _taskIdToViewMap[uniqueKey] = taskView;

            if (task.Progress < task.MaxProgress)
            {
                if (task.Progress <= 0)
                {
                    taskView.transform.SetParent(_done);
                }
                else
                {
                    taskView.transform.SetParent(_inProgress);
                }
            }
        
            task.OnProgressChangedFirstTime += (completedTask) => OnTaskProgressChangedFirstTime(uniqueKey, completedTask);
            task.OnProgressChanged += (changedTask, value) => OnTaskProgressChanged(uniqueKey, changedTask, value);
            task.OnTaskCompleted += (completedTask) => OnTaskCompleted(uniqueKey, completedTask);
        }

        private void OnTaskProgressChangedFirstTime(string uniqueKey, ITask completedTask)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                MoveTask(taskView, _inProgress);
            }
        }

        private void OnTaskProgressChanged(string uniqueKey, ITask task, float value)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                if (taskView != null)
                {
                    //taskView.transform.SetParent(_inProgress);
                    taskView.UpdateProgress(task.Progress, value);
                    taskView.AnimateTextFx(value);
                }
            }
        }

        private void OnTaskCompleted(string uniqueKey, ITask task)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                MoveTask(taskView, _done);
            }
        }

        private void MoveTask(TaskView taskView, Transform to)
        {
            taskView.HideTask( () =>
            {
                taskView.transform.SetParent(to);
                taskView.ShowTask();
            });
        }
        
        public async Task ClearTasks()
        {
            _isBuisy = true;
            _taskViews.Reverse();

            foreach (var taskView in _taskViews)
            {
                await taskView.HideTaskAsync();  
            }

            foreach (var taskView in _taskViews)
            {
                Destroy(taskView.gameObject);
            }

            _isBuisy = false;
            _taskViews.Clear();
            _taskIdToViewMap.Clear();
        }
        
        public bool IsBuisy => _isBuisy;
    }
}