using System;
using System.Collections.Generic;
using _root.Notification;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Messenger
{
    public class JobMessageGenerator : IExecute, ICleanUp
    {
        private CalendarLogic _calendarLogic;
        private LocalEvents _localEvents;
        private readonly MessengerConfig _config;
        private readonly TimeLogic _timeLogic;
        private int _hourToGenerate;
        private int _minuteToGenerate;

        private Dictionary<string, IMessageSender> _messageMap = new();
        private int _maxMessageForGeneration = 5;

        public JobMessageGenerator(CalendarLogic calendarLogic, LocalEvents localEvents, MessengerConfig config, TimeLogic timeLogic)
        {
            _calendarLogic = calendarLogic;
            _localEvents = localEvents;
            _config = config;
            _timeLogic = timeLogic;

            _localEvents.OnNewDay += ScheguleTime;
            _localEvents.OnNewMinute += TryToGenerate;
            _localEvents.OnMessageReaded += RemoveMessageFromMap;
        }

        private void RemoveMessageFromMap(string id)
        {
            _messageMap.Remove(id);
            Debug.Log("Message removed from map. Now -" + _messageMap.Count);
        }

        private void TryToGenerate()
        {
            if(_messageMap.Count >= _maxMessageForGeneration) return;
            
            if(_timeLogic.CurrentHour != _hourToGenerate) return;

            if (_timeLogic.CurrentMinute == _minuteToGenerate)
            {
                var message = new SimpleMessageSender(
                
                    name: "HR1",
                    message: "We have job for you!",
                    onAccept: CreateEvent 
                );

                var notification = new Notification(
                    message.Id, 
                    NotificationType.Message,
                    "You have new message",
                    "You have new message",
                    _calendarLogic.GetCurrentDate().Year,
                    _calendarLogic.GetCurrentDate().Month,
                    _calendarLogic.GetCurrentDate().Day,
                    _hourToGenerate,
                    _minuteToGenerate + 1);
                
                _localEvents.TriggerNewNotificationCreated(notification);
            
                _messageMap.Add(message.Id, message);
                _localEvents.TriggerNewMessageAddToMassanger(message);
            }
        }

        private void CreateEvent()
        {
            var calendarEvent = new CalendarEvent();
            var currentDate = _calendarLogic.GetCurrentDate();
            
            calendarEvent.Id = Guid.NewGuid().ToString();
            calendarEvent.Name = "job interview";
            calendarEvent.Month = currentDate.Month;
            calendarEvent.Day = currentDate.Day + 1;
            calendarEvent.Year = currentDate.Year;
            calendarEvent.Hour = 20;
            calendarEvent.Minute = 0;
            calendarEvent.Message = $"Time to go to {calendarEvent.Name}";
            
            _localEvents.TriggerCalendarEventCreated(calendarEvent);
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
        }

        public void Execute(float deltatime)
        {
        }
    }
}