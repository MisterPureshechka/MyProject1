using System;
using UnityEngine;

namespace Scripts.Tasks
{
    public class ChillTask : IChillTask
    {
        private float _lastUpdateTime;
    
        public string Title { get; }
        public string Id { get; } = Guid.NewGuid().ToString(); // Автогенерация ID
        public float Progress { get; set; }
        public bool IsCompleted { get; private set; }

        public ChillTask(string title, float progress)
        {
            Title = title;
            Progress = progress;
        }
    
        public event Action<ITask> OnTaskCompleted;
        public event Action<ITask, float> OnProgressChanged;
    
        public ITask Clone()
        {
            return new ChillTask(Title, Progress);
        }
    
        public void ApplyProgress(float delta, float interval = 0)
        {
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
            float oldProgress = Progress;
            Progress = Mathf.Max(Progress - delta);
            _lastUpdateTime = Time.time;
        
            if (Progress != oldProgress)
            {
                OnProgressChanged?.Invoke(this, delta);
            }

            if (Progress <= 0 && !IsCompleted)
            {
                IsCompleted = true;
                OnTaskCompleted?.Invoke(this);
            }
        }
    }
}