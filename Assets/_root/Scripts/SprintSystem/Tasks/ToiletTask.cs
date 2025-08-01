using System;
using System.Collections.Generic;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Tasks
{
    public class ToiletTask : IToiletTask
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private float _lastUpdateTime;
        private bool _hasProgressChanged;
        public string Title { get; }
        public string Id { get; }
        public float Progress { get; private set; }
        public float MaxProgress { get; }
        public bool IsCompleted { get; private set; }
        
        private readonly Dictionary<string, Dictionary<string, float>> _effects;

        public event Action<ITask> OnTaskCompleted;

        public event Action<ITask, float, float> OnProgressChanged;

        public event Action<ITask> OnProgressChangedFirstTime;

        public ToiletTask(ProgressDataAdapter progressDataAdapter, string title, float progress)
        {
            _progressDataAdapter = progressDataAdapter;
            Title = title;
            Progress = progress;

            _effects = StatEffectLoader.Load();
        }
        
        public ITask Clone()
        {
            return new ToiletTask(_progressDataAdapter, Title, Progress); 
        }

        public void ApplyProgress(float interval = 0f)
        {
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
            float delta = _progressDataAdapter.GetProgressData().Metadata.GetProgressDelta(SprintType.Toilet.ToString());
            
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