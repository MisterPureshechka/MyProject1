using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _root.Notification;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
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

        private Dictionary<string, IMessageSender> _messageMap = new();
        private int _maxMessageForGeneration = 5;
        
        public JobMessageGenerator(CalendarLogic calendarLogic, LocalEvents localEvents, MessengerConfig config, TimeLogic timeLogic, IComeBackStore comeBackStore)
        {
            _calendarLogic = calendarLogic;
            _localEvents = localEvents;
            _config = config;
            _timeLogic = timeLogic;

            _comeBackStore = comeBackStore;

            ScheguleTime();
            _localEvents.OnNewDay += ScheguleTime;
            _localEvents.OnMessageReaded += RemoveMessageFromMap;
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




        private void ScheguleTime()
        {
            _hourToGenerate = Random.Range(9, 12);
            _minuteToGenerate = Random.Range(0, 60);
        }

        public void CleanUp()
        {
            _localEvents.OnNewDay -= ScheguleTime;
            _localEvents.OnMessageReaded -= RemoveMessageFromMap;
        }
    }
}