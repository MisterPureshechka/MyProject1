using System;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Tasks
{
    public class EatTask : IEatTask
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        
        private float _lastUpdateTime;
        private bool _hasProgressChanged;
        public event Action<ITask> OnTaskCompleted;
        public event Action<ITask, float, float> OnProgressChanged;
        public event Action<ITask> OnProgressChangedFirstTime;
        
        public EatTaskType Type { get; set; }
        
        public string Id { get; private set; }
        public string Title { get; }
        public float Progress { get; set; }
        public float MaxProgress { get; }
        public bool IsCompleted { get; private set; }

        public EatTask(ProgressDataAdapter progressDataAdapter, EatTaskType taskType, string title, float progress)
        {
            _progressDataAdapter = progressDataAdapter;
            Type = taskType;
            Title = title;
            Progress = progress;
            MaxProgress = progress;
            Id = Guid.NewGuid().ToString();
        }
        
        public ITask Clone()
        {
            return new EatTask(_progressDataAdapter, Type, Title, Progress)
            {
                Id = Id  
            };
        }
        
        public void ApplyProgress(float interval = 0f)
        {
            interval = 0.5f;
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
            float delta = 1;
            
            float oldProgress = Progress;
            Progress = Math.Max(0, Progress - delta);
            _lastUpdateTime = Time.time;
            
            if (Progress != oldProgress)
            {
                if (!_hasProgressChanged)
                {
                    _hasProgressChanged = true;
                    OnProgressChangedFirstTime?.Invoke(this);
                }
                else
                {
                    OnProgressChanged?.Invoke(this, delta, interval);
                }
            }
        
            if (Progress <= 0 && !IsCompleted)
            {
                IsCompleted = true;
                OnTaskCompleted?.Invoke(this);
            }
        }
    }
}