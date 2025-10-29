using System;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CleanTask : ICleanTask
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private float _lastUpdateTime;
        private bool _hasProgressChanged;
        
        public event Action<ITask> OnTaskCompleted;
        public event Action<ITask, float, float> OnProgressChanged;
        public event Action<ITask> OnProgressChangedFirstTime;
        public string Title { get; }
        public string Id { get; } = Guid.NewGuid().ToString(); 
        public float Progress { get; set; }
        public float MaxProgress { get; }
        public bool IsCompleted { get; private set; }

        public CleanTask(ProgressDataAdapter progressDataAdapter, string title, float progress)
        {
            _progressDataAdapter = progressDataAdapter;
            Title = title;
            Progress = progress;
            MaxProgress = progress;
        }
    
        public ITask Clone()
        {
            return new CleanTask(_progressDataAdapter, Title, Progress); 
        }
    
        public void ApplyProgress(float interval = 0f)
        {
            interval = 1f;
            
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
            //float delta = _progressDataAdapter.GetProgressData().Metadata.GetProgressDelta(SprintType.Clean.ToString());
            float delta = 10f;
            
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