using System;
using System.Collections.Generic;
using Scripts.Bugs;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Tasks
{
    public class DevTask : IDevTask
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly BugLogic _bugLogic;
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

        public DevTask(ProgressDataAdapter progressDataAdapter, BugLogic bugLogic, DevTaskType taskType, string title, float progress)
        {
            _progressDataAdapter = progressDataAdapter;
            _bugLogic = bugLogic;
            Type = taskType;
            Title = title;
            Progress = progress;
            MaxProgress = progress;
            Id = Guid.NewGuid().ToString();
        }

        public ITask Clone()
        {
            return new DevTask(this._progressDataAdapter, this._bugLogic, this.Type, this.Title, this.Progress)
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
                    //HasChanceForBug = _bugLogic.TryRollBugStart(MaxProgress, out var emitAt);
                    HasChanceForBug = true;
                    if (HasChanceForBug) ProgressToEmitBug = 50f;

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
                    //_bugLogic.RollBugResult(out var resultValue, out var success);
                    Result = 4;                          
                    //OnBugResult?.Invoke(this, resultValue, success);
                    OnBugResult?.Invoke(this, Result, true);
                }
                else
                {
                    Result = 1;
                }
                
                SetBug(false);
                OnTaskCompleted?.Invoke(this);
            }
        }
        
        private int GetBugResult(out bool success)
        {
            success = Random.value >= 0.5f;       
            return success ? Random.Range(3, 6)   
                : 1;
        }

        private bool TryEmitBug()
        {
            return true;
        }

        private void SetBug(bool isBug)
        {
            IsBug = false; 
            BugStateChanged?.Invoke(IsBug);
            return;
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
        public void RestoreFromSnapshot(DevTaskSnapshot s)
        {
            if (!string.IsNullOrEmpty(s.Id)) this.Id = s.Id;

            this.Progress          = s.Progress;

            this.IsCompleted       = s.IsCompleted;

            this.IsBug             = s.IsBug;              
            this.HasChanceForBug   = s.HasChanceForBug;
            this.ProgressToEmitBug = s.ProgressToEmitBug;
            this.Result            = s.Result;

            this._hasProgressChanged = s.HasProgressChanged;
        }


    }
}
