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

            ScheguleTime();
            _localEvents.OnNewDay += ScheguleTime;
            _localEvents.OnNewMinute += TryToGenerate;
            _localEvents.OnMessageReaded += RemoveMessageFromMap;
            TryLoadEvents();
        }
        
        private async void TryLoadEvents()
        {
            var records = _comeBackStore.PullAll();
            if (records == null || records.Count == 0) return;

            await Task.Delay(1000);

            var readIds = new HashSet<string>();
            foreach (var r in records)
                if (r.EventType == CalendarEventType.JobMessageRead && !string.IsNullOrEmpty(r.EventId))
                    readIds.Add(r.EventId);

            foreach (var r in records)
            {
                if (r.EventType != CalendarEventType.JobOffer) continue;
                if (string.IsNullOrEmpty(r.EventId) || readIds.Contains(r.EventId)) continue;

                var devJob = _jobLogic.FindJob(r.CompanyName, r.JobTitle);
                if (devJob == null)
                {
                    Debug.LogWarning($"[JobMsg] Job not found: {r.CompanyName} / {r.JobTitle}");
                    continue;
                }

                var daysText = string.Join(", ", devJob.SalaryDays ?? Array.Empty<int>());
                var msgText =
                    $"Congratulations! {devJob.CompanyName} offers you a {devJob.JobTitle} position.\n" +
                    $"Salary: {devJob.Salary}$ (paid on {daysText} days of each month).\n" +
                    $"Start at {devJob.WorkStartTime}:00.\n\n" +
                    $"{devJob.HoursBeforeComeBack} hours working day.\n\n" +
                    $"Accept to join {devJob.CompanyName}.";

                var msg = new SimpleMessageSender(
                    name: devJob.HRName ?? "HR",
                    message: msgText,
                    onAccept: () => _jobLogic.GetJob(devJob)
                )
                {
                    Id = r.EventId
                };

                int y = r.Year, m = r.Month, d = r.Day;
                int h = r.Hour, min = r.Minute + 1;
                Normalize(ref y, ref m, ref d, ref h, ref min);

                var notif = new Notification(
                    msg.Id, NotificationType.Message,
                    "Job offer", $"Offer from {devJob.CompanyName}",
                    y, m, d, h, min
                );

                _localEvents.TriggerNewNotificationCreated(notif);
                _messageMap[msg.Id] = msg;
                _localEvents.TriggerNewMessageAddToMassanger(msg);
            }
        }



        private void Normalize(ref int y, ref int m, ref int d, ref int h, ref int min)
        {
            if (min >= 60) { min -= 60; h += 1; }
            if (h >= 24)   { h -= 24;  d += 1; }
        }


        private void AppendOfferRecord(IJob job, IMessageSender message)
        {
            var now = _calendarLogic.GetCurrentDate();

            _comeBackStore.Append(new ComeBackRecord
            {
                EventId = message.Id,
                EventType = CalendarEventType.JobOffer,  
                CompanyName = job.CompanyName,
                HRName = job.HRName,
                JobId = MakeJobId(job), 
                JobTitle = job.JobTitle,
                Salary = job.Salary,
                SalaryDays = job.SalaryDays,
                WorkStart = job.WorkStartTime,
                Message = message.Message,
                Year = now.Year, Month = now.Month, Day = now.Day,
                Hour = _hourToGenerate, Minute = _minuteToGenerate,
                HoursBeforeComeBack = job.HoursBeforeComeBack,
            });
        }
        
        private static string MakeJobId(IJob job) =>
            $"{job.CompanyName}|{job.JobTitle}".ToLowerInvariant().Replace(" ", "_");

        private void RemoveMessageFromMap(string id)
        {
            _messageMap.Remove(id);
            Debug.Log("Message removed from map. Now -" + _messageMap.Count);

            _comeBackStore.Append(new ComeBackRecord
            {
                EventId = id,
                EventType = CalendarEventType.JobMessageRead,
                Year = _calendarLogic.GetCurrentDate().Year,
                Month = _calendarLogic.GetCurrentDate().Month,
                Day = _calendarLogic.GetCurrentDate().Day,
                Hour = _timeLogic.CurrentHour,
                Minute = _timeLogic.CurrentMinute,
            });
        }


        private void TryToGenerate()
        {
            if (_messageMap.Count >= _maxMessageForGeneration) return;
            if (_timeLogic.CurrentHour != _hourToGenerate) return;
            if (_timeLogic.CurrentMinute != _minuteToGenerate) return;

            var job = _jobLogic.LoadJob();
            _lastOfferedJob = job;

            var daysText = string.Join(", ", job.SalaryDays ?? Array.Empty<int>());
            var msgText =
                $"Congratulations! {job.CompanyName} offers you a {job.JobTitle} position.\n" +
                $"Salary: {job.Salary}$ (paid on {daysText} days of each month).\n" +
                $"Start at {job.WorkStartTime}:00.\n\n" +
                $"{job.HoursBeforeComeBack} hours working day.\n\n" +
                $"We will be happy you to join {job.CompanyName}!";

            var message = new SimpleMessageSender(
                name: job.HRName,
                message: msgText,
                onAccept: () => _jobLogic.GetJob((IDevJob)job)   
            );

            var now = _calendarLogic.GetCurrentDate();
            var notification = new Notification(
                message.Id,
                NotificationType.Message,
                "Job offer",
                $"Offer from {job.CompanyName}",
                now.Year, now.Month, now.Day,
                _hourToGenerate, _minuteToGenerate + 1
            );

            _localEvents.TriggerNewNotificationCreated(notification);

            _messageMap[message.Id] = message;
            _localEvents.TriggerNewMessageAddToMassanger(message);

            AppendOfferRecord(job, message);
        }


        private void ScheguleTime()
        {
            _hourToGenerate = Random.Range(9, 12);
            _minuteToGenerate = Random.Range(0, 60);
        }

        public void CleanUp()
        {
            _localEvents.OnNewDay -= ScheguleTime;
            _localEvents.OnNewMinute -= TryToGenerate;
            _localEvents.OnMessageReaded -= RemoveMessageFromMap;
        }
    }
}