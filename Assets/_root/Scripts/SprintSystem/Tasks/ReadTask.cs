using System;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Tasks
{
    public class ReadTask : IReadTask
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;
        public DevTaskType _knowledgeToUpgrade;
        public DevTaskType KnowledgeToUpgrade => _knowledgeToUpgrade;
        private float _lastUpdateTime;
        private bool _hasProgressChanged;
        public string Title { get; }
        public string Id { get; }
        public float Progress { get; private set; }
        public float MaxProgress { get; }
        public bool IsCompleted { get; private set; }

        public event Action<ITask> OnTaskCompleted;

        public event Action<ITask, float, float> OnProgressChanged;

        public event Action<ITask> OnProgressChangedFirstTime;

        public ReadTask(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents, DevTaskType knowledgeToUpgrade, string title, float progress)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            _knowledgeToUpgrade = knowledgeToUpgrade;
            Title = title;
            Progress = progress;
        }
        
        public ITask Clone()
        {
            return new ReadTask(_progressDataAdapter, _localEvents, _knowledgeToUpgrade, Title, Progress); 
        }

        public void ApplyProgress(float interval = 0f)
        {
            if (Time.time - _lastUpdateTime < interval) 
                return;
            
            float delta = _progressDataAdapter.GetProgressData().Metadata.GetProgressDelta(_knowledgeToUpgrade.ToString());
            
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

            if (!IsCompleted) _localEvents.TriggerReadTaskUpdate(_knowledgeToUpgrade);
        }
    }
}