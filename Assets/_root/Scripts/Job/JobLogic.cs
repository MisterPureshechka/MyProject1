using System;
using System.Collections.Generic;
using Core;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Job
{
    public class JobLogic : IExecute
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private IJob _currentJob;
        private JobLibrary _jobLibrary;
        private readonly LocalEvents _localEvents;
        private readonly CalendarLogic _calendarLogic;
        
        private const string CurrentJobIndexKey = "CurrentJobIndex";

        private GameDate _nextPayday;
        
        public JobLogic(ProgressDataAdapter progressDataAdapter, JobLibrary jobLibrary, LocalEvents localEvents, CalendarLogic calendarLogic)
        {
            _progressDataAdapter = progressDataAdapter;
            _jobLibrary = jobLibrary;
            _localEvents = localEvents;
            _calendarLogic = calendarLogic;
            _localEvents.OnNewDay += CheckDayOfSalary;
            
            TryLoadJob();
        }

        private void TryLoadJob()
        {
            var meta = _progressDataAdapter.GetProgressData().Metadata;
            if (!meta.TryGetValue(CurrentJobIndexKey, out var data)) return;

            int index = Mathf.RoundToInt(data.Value);
            var jobs = _jobLibrary.GetDevJobs();
            if (index >= 0 && index < jobs.Count)
            {
                _currentJob = jobs[index];
                _localEvents.TriggerNewJobFound(_currentJob as IDevJob);
                Debug.Log($"[Job] Loaded by index {index}: {_currentJob.JobTitle}");
            }
        }
        
        private void SaveJobIndex(IDevJob job)
        {
            var jobs = _jobLibrary.GetDevJobs();
            int idx = jobs.IndexOf(job as DevJob); 
            if (idx < 0) return;

            if (_progressDataAdapter.GetProgressData().Metadata.TryGetValue(CurrentJobIndexKey, out var data))
                data.Value = idx;
        }
        
        private void CheckDayOfSalary()
        {
            if (_currentJob == null) return;

            var today = _calendarLogic.GetCurrentDate();

            foreach (var day in _currentJob.SalaryDays)
            {
                if (today.Day == day)
                {
                    _localEvents.TriggerIncreaseWalletAmount(_currentJob.Salary);
                }
            }
        }

        public void TryGetJob(IDevJob job)
        {
            var knowledge = GetKnowledge();
            if (job.TryGetJob(knowledge))
            {
                _currentJob = job;
                _localEvents.TriggerNewJobFound(job);
                Debug.Log($"Job found: {job.JobTitle}");
            }
            else
            {
                Debug.LogError($"Job not found: {job.JobTitle}");
            }
        }

        private Dictionary<DevTaskType, float> GetKnowledge()
        {
            var meta = _progressDataAdapter.GetProgressData().Metadata;
            var result = new Dictionary<DevTaskType, float>();

            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
            {
                result[type] = meta.GetValue(type.ToString());
            }

            return result;
        }

        public IJob LoadJob()
        {
            return _jobLibrary.GetDevJob();
        }

        public void Execute(float deltatime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int randomIndex = Random.Range(0, _jobLibrary.GetDevJobs().Count); 
                var job = _jobLibrary.GetDevJobs()[randomIndex];
                TryGetJob(job);
            }
        }

        public void GetJob(IDevJob newJob)
        {
            _currentJob = newJob;
            _localEvents.TriggerNewJobFound(newJob);
            SaveJobIndex(newJob);
        }
    }
}