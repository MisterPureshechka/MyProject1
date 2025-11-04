using System;
using System.Collections.Generic;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        public event Action<ITask, int, bool> OnBugResult;

        public bool HasChanceForBug;
        public int Result { get; private set; }
        public event Action<bool> BugStateChanged;
        public bool IsBug { get; private set; }       
        
        public float ProgressToEmitBug;                      

        public DevTaskType Type { get; set; }
        public event Action OnBugEmit;

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
                    HasChanceForBug = TryEmitBug();
                    if (HasChanceForBug)
                        ProgressToEmitBug = Random.Range(0f, MaxProgress * 0.5f);

                    _hasProgressChanged = true;
                    OnProgressChangedFirstTime?.Invoke(this);
                }
                else
                {
                    OnProgressChanged?.Invoke(this, delta, interval);
                }
            }

            if (!IsBug && HasChanceForBug && Progress <= ProgressToEmitBug)
            {
                SetBug(true);
                OnBugEmit?.Invoke(); 
            }
        
            if (Progress <= 0 && !IsCompleted)
            {
                IsCompleted = true;

                if (IsBug)
                {
                    bool success;
                    int resultValue = GetBugResult(out success);
                    Debug.Log("Bug result - " + resultValue);
                    Result = resultValue;
                    OnBugResult?.Invoke(this, resultValue, success);
                }

                SetBug(false);
                OnTaskCompleted?.Invoke(this);
            }
        }
        
        private int GetBugResult(out bool success)
        {
            success = Random.value >= 0.5f;       // 50% на успех
            return success ? Random.Range(3, 6)   // 3..5
                : 1;
        }

        private bool TryEmitBug()
        {
            return true;
        }

        private void SetBug(bool isBug)
        {
            if (IsBug == isBug) return;
            IsBug = isBug;

            if (IsBug) Progress *= 3f;

            BugStateChanged?.Invoke(IsBug);

        }
        
        public DevTaskSnapshot ToSnapshot()
        {
            return new DevTaskSnapshot
            {
                Id                = this.Id,
                DevType           = this.Type.ToString(),
                Title             = this.Title,
                Progress          = this.Progress,
                MaxProgress       = this.MaxProgress,
                IsCompleted       = this.IsCompleted,
                IsBug             = this.IsBug,
                HasChanceForBug   = this.HasChanceForBug,
                ProgressToEmitBug = this.ProgressToEmitBug,
                Result            = this.Result,
                HasProgressChanged= this._hasProgressChanged
            };
        }

// ВАЖНО: только прямые присваивания, никаких событий, никаких SetBug(true)!
        public void RestoreFromSnapshot(DevTaskSnapshot s)
        {
            if (!string.IsNullOrEmpty(s.Id)) this.Id = s.Id;

            this.Progress          = s.Progress;
            // MaxProgress/Title/Type уже соответствуют прототипу; если хочешь — доверься сейву:
            // this.MaxProgress    = s.MaxProgress;
            // this.Title          = s.Title;
            // this.Type           = Enum.Parse<DevTaskType>(s.DevType);

            this.IsCompleted       = s.IsCompleted;

            this.IsBug             = s.IsBug;              // без SetBug — чтобы не умножать прогресс и не шлёпать события
            this.HasChanceForBug   = s.HasChanceForBug;
            this.ProgressToEmitBug = s.ProgressToEmitBug;
            this.Result            = s.Result;

            this._hasProgressChanged = s.HasProgressChanged;
        }


    }
}
