using System;
using System.Collections.Generic;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Tasks
{
    public class DevTask : IDevTask
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private float _lastUpdateTime;
        private bool _hasProgressChanged;
        private Dictionary<string,Dictionary<string,float>> _effects;
        public event Action<ITask> OnTaskCompleted;
        public event Action<ITask, float, float> OnProgressChanged;
        public event Action<ITask> OnProgressChangedFirstTime;

        public DevTaskType Type { get; set; }

        public string Id { get; private set;}
        public string Title { get; }
        public float Progress { get; set; }
        public float MaxProgress { get; }
        public bool IsCompleted { get; private set; }

        public DevTask(ProgressDataAdapter progressDataAdapter, DevTaskType taskType, string title, float progress)
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
            return new DevTask(this._progressDataAdapter, this.Type, this.Title, this.Progress)
            {
                Id = this.Id  
            };
        }

        public void ApplyProgress(float interval = 0f)
        {
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
            float delta = _progressDataAdapter.GetProgressData().Metadata.GetValue(Type.ToString());
            
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