using System;
using System.Collections.Generic;
using _root.Notification;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Stat;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Job
{
    public class JobLogic : IController
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private IJob _currentJob;
        private JobLibrary _jobLibrary;
        private readonly LocalEvents _localEvents;
        private readonly CalendarLogic _calendarLogic;
        private readonly TimeLogic _timeLogic;

        private const string CurrentJobIndexKey = "CurrentJobIndex";
        
        private ExitEvent _jobExitEvent;

        private GameDate _nextPayday;
        
        public JobLogic(ProgressDataAdapter progressDataAdapter, JobLibrary jobLibrary, LocalEvents localEvents, CalendarLogic calendarLogic, TimeLogic timeLogic)
        {
            _progressDataAdapter = progressDataAdapter;
            _jobLibrary = jobLibrary;
            _localEvents = localEvents;
            _calendarLogic = calendarLogic;
            _timeLogic = timeLogic;
            _localEvents.OnNewDay += CheckDayOfSalary;
            _localEvents.OnNewHour += CheckTimeToGoToWork;
            
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
        
        private void CheckTimeToGoToWork()
        {
            if (_currentJob == null) return;
            if(_calendarLogic.IsWeekend()) return;
            
            var date = _calendarLogic.GetCurrentDate();

            if (_timeLogic.CurrentHour == _currentJob.WorkStartTime)
            {
                
                var exitEvent = new ExitEvent
                {
                    EventTime = _currentJob.WorkStartTime,
                    HoursBeforeComeBack = _currentJob.HoursBeforeComeBack,
                    HealthToUpdateAfter = _currentJob.HealthToUpdateAfter,
                    KnowledgeToUpdateAfter = _currentJob.KnowledgeToUpdateAfter,
                };
                
                _localEvents.TriggerExitEventCreated(exitEvent);
                
                var notification = new Notification(Guid.NewGuid().ToString(), NotificationType.Calendar, "Work started", "Time to go to work", date.Year, date.Month,  date.Day, _timeLogic.CurrentHour, _timeLogic.CurrentMinute);
                _localEvents.TriggerNewNotificationCreated(notification);
            }
        }

        public IJob LoadJob()
        {
            return _jobLibrary.GetDevJob();
        }

        public void GetJob(IDevJob newJob)
        {
            _currentJob = newJob;
            _localEvents.TriggerNewJobFound(newJob);
            SaveJobIndex(newJob);
        }

        public IDevJob FindJob(string companyName, string jobTitle)
        {
            var jobs = _jobLibrary.GetDevJobs();
            for (int i = 0; i < jobs.Count; i++)
            {
                var j = jobs[i];
                if (j.CompanyName == companyName && j.JobTitle == jobTitle)
                    return j;
            }
            return null;
        }
    }

    public class ExitEvent
    {
        public string EventTitle;
        public int EventTime;
        public int HoursBeforeComeBack;
        public Dictionary<DevTaskType, float> KnowledgeToUpdateAfter;
        public Dictionary<HealthStatType, float> HealthToUpdateAfter;
    }
}