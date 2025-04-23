using System;
using UnityEngine;

namespace Scripts.Tasks
{
    public class EatTask : IEatTask
    {
        private float _lastUpdateTime;
        public event Action<ITask> OnTaskCompleted;
        public event Action<ITask, float> OnProgressChanged;
        public EatTaskType Type { get; set; }

        public string Id { get; private set; }
        public string Title { get; }
        public float Progress { get; set; }
        public bool IsCompleted { get; private set; }

        public EatTask(EatTaskType taskType, string title, float progress)
        {
            Type = taskType;
            Title = title;
            Progress = progress;
        }
        
        public ITask Clone()
        {
            return new EatTask(this.Type, this.Title, this.Progress)
            {
                Id = this.Id  
            };
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
                OnProgressChanged?.Invoke(this, delta);
            }
        
            if (Progress <= 0)
            {
                IsCompleted = true;
                OnTaskCompleted?.Invoke(this);
            }
        }
    }
}