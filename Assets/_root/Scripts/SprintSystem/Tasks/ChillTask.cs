using System;
using UnityEngine;

namespace Scripts.Tasks
{
    public class ChillTask : IChillTask
    {
        private float _lastUpdateTime;
        private bool _hasProgressChanged;
        public event Action<ITask> OnTaskCompleted;
        public event Action<ITask, float> OnProgressChanged;
        public event Action<ITask> OnProgressChangedFirstTime;
        public string Title { get; }
        public string Id { get; } = Guid.NewGuid().ToString(); // Автогенерация ID
        public float Progress { get; set; }
        public float MaxProgress { get; }
        public bool IsCompleted { get; private set; }

        public ChillTask(string title, float progress)
        {
            Title = title;
            Progress = progress;
            MaxProgress = progress;
        }
    
        public ITask Clone()
        {
            return new ChillTask(Title, Progress);
        }
    
        public void ApplyProgress(float delta, float interval = 0f)
        {
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
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
                    OnProgressChanged?.Invoke(this, delta);
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