using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class SprintView : MonoBehaviour
    {
        private readonly List<TaskView> _taskViews = new();
        private readonly Dictionary<string, TaskView> _taskIdToViewMap = new();

        private readonly Dictionary<ITask, Action<bool>> _bugHandlers = new();
        private readonly Dictionary<ITask, Action<ITask,int,bool>> _bugResultHandlers = new();

        [field: SerializeField] public Transform ToDoField;
        [SerializeField] private Transform _inProgress;
        [SerializeField] private Transform _done;
        [SerializeField] private Vector3 _shakeValue;

        private bool _isBuisy;
        private Tweener _currentTween;
        private LocalEvents _localEvents;

        public void Init(LocalEvents localEvents) => _localEvents = localEvents;

        public async Task AddTask(ITask task, TaskView taskView, SprintType sprintType)
        {
            while (_isBuisy)
                await Task.Yield();

            string uniqueKey = $"{task.Id}_{Guid.NewGuid()}";

            if (task is IDevTask devTask)
            {
                taskView.SetInfoIfDev(task.Title, task.Progress, devTask.Type, _localEvents);
                taskView.SetBugVisual(devTask.IsBug);
                taskView.ShowTask();

                if (task.IsCompleted && devTask is DevTask dt && dt.Result > 0)
                {
                    taskView.SetBugVisual(false);
                    if (dt.Result > 1) taskView.ShowExtraSprite(Mathf.Clamp(dt.Result, 1, 5));
                    else taskView.SetUnsuccessTask(); 
                }
                else
                {
                    taskView.SetBugVisual(devTask.IsBug);
                }

                void OnBug(bool isBug)
                {
                    if (taskView != null)
                        taskView.SetBugVisual(isBug);
                }
                devTask.BugStateChanged += OnBug;
                _bugHandlers[task] = OnBug;

                Action<ITask,int,bool> onBugResult = (t, value, success) =>
                {
                    if (taskView == null) return;

                    if (success)
                    {
                        taskView.ShowExtraSprite(Mathf.Clamp(value, 1, 5)); // 3..5
                    }
                    else
                    {
                        Debug.Log($"Should be black card");
                        taskView.SetUnsuccessTask();
                    }

                    Debug.Log($"[SprintView] Bug result '{t.Title}': value={value}, success={success}");
                };
                
                devTask.OnBugResult += onBugResult;
                _bugResultHandlers[task] = onBugResult;

                task.OnTaskCompleted += async _ =>
                {
                    if (_taskIdToViewMap.TryGetValue(uniqueKey, out var tv))
                    {
                        // var delay = tv.GetExtrasShowDuration();
                        // if (delay > 0f) await Task.Delay(TimeSpan.FromSeconds(delay));

                        MoveTask(tv, _done);
                        tv.StopFx();
                    }
                };

            }
            else
            {
                taskView.SetInfo(task.Title, task.Progress, sprintType, _localEvents);
                taskView.ShowTask();

                task.OnTaskCompleted += _ =>
                {
                    if (_taskIdToViewMap.TryGetValue(uniqueKey, out var tv))
                    {
                        MoveTask(tv, _done);
                        tv.StopFx();
                    }
                };
            }

            _taskViews.Add(taskView);
            _taskIdToViewMap[uniqueKey] = taskView;

            if (task.Progress < task.MaxProgress)
            {
                taskView.transform.SetParent(task.Progress <= 0 ? _done : _inProgress);
            }

            task.OnProgressChangedFirstTime += _ => OnTaskProgressChangedFirstTime(uniqueKey, task);
            task.OnProgressChanged += async (_, value, interval) => OnTaskProgressChanged(uniqueKey, task, value, interval);
            task.OnTaskCompleted += _ =>
            {
                if (_taskIdToViewMap.TryGetValue(uniqueKey, out var tv))
                {
                    MoveTask(tv, _done);
                    tv.StopFx();
                }
            };
        }

        private void OnTaskProgressChangedFirstTime(string uniqueKey, ITask completedTask)
        {
            if (_taskIdToViewMap.TryGetValue(uniqueKey, out var taskView))
                MoveTask(taskView, _inProgress);
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
                    if (task.IsCompleted) taskView.StopFx();
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
            taskView.HideTask(() =>
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
                // Снимаем ВСЕ подписки один раз
                foreach (var kv in _bugHandlers)
                    if (kv.Key is IDevTask d) d.BugStateChanged -= kv.Value;
                _bugHandlers.Clear();

                foreach (var kv in _bugResultHandlers)
                    if (kv.Key is IDevTask d) d.OnBugResult -= kv.Value;
                _bugResultHandlers.Clear();

                // Прячем карточки плавно (в обратном порядке)
                _taskViews.Reverse();
                foreach (var taskView in _taskViews)
                    await taskView.HideTaskAsync();

                // Уничтожаем UI-объекты
                foreach (var taskView in _taskViews)
                    Destroy(taskView.gameObject);

                // Чистим коллекции
                _taskViews.Clear();
                _taskIdToViewMap.Clear();

                // Гасим активный твинап, если есть
                _currentTween?.Kill();
                _currentTween = null;
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
