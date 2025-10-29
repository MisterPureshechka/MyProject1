using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private Vector3 _shakeValue;
        private bool _isBuisy;
        private Tweener _currentTween;

        public async Task AddTask(ITask task, TaskView taskView, SprintType sprintType)
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
                taskView.SetInfo(task.Title, task.Progress, sprintType);
                taskView.ShowTask();
            }
            
            Debug.LogWarning($"task is - {task.Title}");
            
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
            task.OnProgressChanged += (changedTask, value, interval) => OnTaskProgressChanged(uniqueKey, changedTask, value, interval);
            task.OnTaskCompleted += (completedTask) => OnTaskCompleted(uniqueKey, completedTask);
        }

        private void OnTaskProgressChangedFirstTime(string uniqueKey, ITask completedTask)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                MoveTask(taskView, _inProgress);
            }
        }

        private async void OnTaskProgressChanged(string uniqueKey, ITask task, float value, float interval)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                if (taskView != null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(interval));
                    taskView.UpdateProgress(task.Progress, value);
                    taskView.AnimateTextFx(value, interval);
                    if(task.IsCompleted) taskView.StopFx();
                }
            }
        }

        private void OnTaskCompleted(string uniqueKey, ITask task)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
            {
                MoveTask(taskView, _done);
                taskView.StopFx();
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
            try
            {
                _taskViews.Reverse();
                foreach (var taskView in _taskViews)
                    await taskView.HideTaskAsync();

                foreach (var taskView in _taskViews)
                    Destroy(taskView.gameObject);

                _taskViews.Clear();
                _taskIdToViewMap.Clear();
            }
            finally
            {
                _isBuisy = false;
            }
        }
        
        public bool IsBuisy => _isBuisy;

        public Tweener currentTween
        {
            get => _currentTween;
            set => _currentTween = value;
        }
    }
}