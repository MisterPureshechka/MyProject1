using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _root.Notification;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using Scripts.Messenger.ComeBackLogic;
using Scripts.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Messenger
{
    public class JobMessageGenerator : ICleanUp
    {
        private CalendarLogic _calendarLogic;
        private LocalEvents _localEvents;
        private readonly MessengerConfig _config;
        private readonly TimeLogic _timeLogic;
        private int _hourToGenerate;
        private int _minuteToGenerate;
        private IComeBackStore _comeBackStore;
        private readonly JobLogic _jobLogic;

        private Dictionary<string, IMessageSender> _messageMap = new();
        private int _maxMessageForGeneration = 5;
        
        private IJob _lastOfferedJob;
        private IJob _lastInterviewJob;

        public JobMessageGenerator(CalendarLogic calendarLogic, LocalEvents localEvents, MessengerConfig config, TimeLogic timeLogic, IComeBackStore comeBackStore, JobLogic jobLogic)
        {
            _calendarLogic = calendarLogic;
            _localEvents = localEvents;
            _config = config;
            _timeLogic = timeLogic;

            _comeBackStore = comeBackStore;
            _jobLogic = jobLogic;

            _localEvents.OnNewDay += ScheguleTime;
            _localEvents.OnNewMinute += TryToGenerate;
            _localEvents.OnMessageReaded += RemoveMessageFromMap;
            _localEvents.OnExitEventType += ExitRoomListener;
            
            TryLoadEvents();
        }
        
        private async void TryLoadEvents()
        {
            var records = _comeBackStore.PullAll();
            if (records == null || records.Count == 0) return;

            await Task.Delay(1000);

            foreach (var record in records)
            {
                if (record.EventType != CalendarEventType.JobInterview) continue;

                var daysText = string.Join(", ", record.SalaryDays);

                string text = record.Success
                    ? $"Congratulations! {record.CompanyName} offers you a {record.JobTitle} position.\n" +
                      $"Salary: {record.Salary}$ (paid on {daysText} days of each month).\n" +
                      $"Start at {record.WorkStart}:00."
                    : $"Thanks for your time. {record.CompanyName} ({record.HRName}) will keep your CV.";

                int y = record.Year, m = record.Month, d = record.Day;
                int h = record.Hour, min = record.Minute + 1;
                Normalize(ref y, ref m, ref d, ref h, ref min);

                var devJob = BuildJobFromRecord(record);

                var message = new ScheduleMessageSender
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = record.HRName,
                    Message = text,
                    Year = y, Month = m, Day = d, Hour = h, Minute = min,
                    OnAccept = record.Success ? () => _jobLogic.GetJob(devJob) : null 
                };
                
                
                var notif = new Notification(
                    message.Id,
                    NotificationType.Message,
                    $"New message from {record.CompanyName}",
                    $"New message from {record.CompanyName}",
                    y, m, d, h, m
                );
            
                _localEvents.TriggerNewNotificationCreated(notif);
                _localEvents.TriggerScheduleMessageAdded(message);
            }
        }
        
        private IDevJob BuildJobFromRecord(ComeBackRecord record)
        {
            return new DevJob(
                record.CompanyName,
                record.HRName,
                record.JobTitle,
                record.Salary,
                record.SalaryDays,
                $"{record.JobTitle} at {record.CompanyName}",
                record.WorkStart,
                null 
            );
        }


        private void Normalize(ref int y, ref int m, ref int d, ref int h, ref int min)
        {
            if (min >= 60) { min -= 60; h += 1; }
            if (h >= 24)   { h -= 24;  d += 1; }
        }


        private void RemoveMessageFromMap(string id)
        {
            _messageMap.Remove(id);
            Debug.Log("Message removed from map. Now -" + _messageMap.Count);
        }

        private void TryToGenerate()
        {
            if (_messageMap.Count >= _maxMessageForGeneration) return;
            if (_timeLogic.CurrentHour != _hourToGenerate) return;
            if (_timeLogic.CurrentMinute != _minuteToGenerate) return;

            var job = _jobLogic.LoadJob();          
            _lastOfferedJob = job;

            var msgText =
                $"Hi! I am {job.HRName} from {job.CompanyName}. \n" +
                $"We have an opened {job.JobTitle} position.\n" +
                $"Do you want to visit our office tomorrow?";

            var message = new SimpleMessageSender(
                name: $"{job.HRName}",
                message: msgText,
                onAccept: () => CreateJobInterview(job)  
            );

            var now = _calendarLogic.GetCurrentDate();
            var notification = new Notification(
                message.Id,
                NotificationType.Message,
                "New job message",
                $"Offer from {job.CompanyName}",
                now.Year, now.Month, now.Day,
                _hourToGenerate, _minuteToGenerate + 1
            );

            _localEvents.TriggerNewNotificationCreated(notification);
            _messageMap.Add(message.Id, message);
            _localEvents.TriggerNewMessageAddToMassanger(message);
        }


        private void CreateJobInterview(IJob job)
        {
            _lastInterviewJob = job;

            var currentDate = _calendarLogic.GetCurrentDate();
            var calendarEvent = new CalendarEvent
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"Interview interview {job.CompanyName}",
                Month = currentDate.Month,
                Day = currentDate.Day + 1,
                Year = currentDate.Year,
                Hour = 20,
                Minute = 0,
                Message = $"Time to go to the interview at {job.CompanyName}",
                ComeBackMessage = "" // заполним при возврате
            };

            _localEvents.TriggerCalendarEventCreated(calendarEvent);
        }
        
        private void ExitRoomListener(CalendarEventType eventType)
        {
            if(eventType != CalendarEventType.JobInterview) return;

            CheckSkillsToGetJob();
        } 

        private void CheckSkillsToGetJob()
        {
            var now = _calendarLogic.GetCurrentDate();
            var job = _lastInterviewJob; 

            bool success = true;

            int y = now.Year, m = now.Month, d = now.Day + Random.Range(1, 3); 
            int h = _timeLogic.CurrentHour + Random.Range(1, 3);
            int min = _timeLogic.CurrentMinute + Random.Range(1, 30);
            Normalize(ref y, ref m, ref d, ref h, ref min);

            _comeBackStore.Append(new ComeBackRecord
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = CalendarEventType.JobInterview,
                Success = success,
                Year = y, Month = m, Day = d, Hour = h, Minute = min,

                CompanyName = job.CompanyName,
                HRName = job.HRName,
                JobTitle = job.JobTitle,
                Salary = job.Salary,
                SalaryDays = job.SalaryDays,
                WorkStart = job.WorkStartTime
            });
        }


        private void ScheguleTime()
        {
            _hourToGenerate = Random.Range(12, 18);
            _minuteToGenerate = Random.Range(0, 60);
        }

        public void CleanUp()
        {
            _localEvents.OnNewDay -= ScheguleTime;
            _localEvents.OnNewMinute -= TryToGenerate;
            _localEvents.OnMessageReaded -= RemoveMessageFromMap;
            _localEvents.OnExitEventType -= ExitRoomListener;
        }
    }
}