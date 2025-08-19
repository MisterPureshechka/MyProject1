using System.Collections.Generic;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Messenger;
using UnityEngine;

namespace _root.Notification
{
    public class NotificationSystem : IExecute, ICleanUp
    {
        private readonly NotificationLibrary _notificationLibrary;
        private readonly CalendarLogic _calendarLogic;
        private readonly List<Notification> _notifications = new();
        private readonly LocalEvents _localEvents;
        private readonly TimeLogic _timeLogic;
        private readonly NotificationView _notificationView;
        private float _timer;
        private bool _isTimerRunning;
        private float _timeBeforeHideNotification = 10f;
        
        private Dictionary<NotificationType, Vector2> _notificationPosition = new();
        
        private MiniCalendarView _miniCalendarView;
        private MiniMessageButton _miniMessageButton;
            

        public NotificationSystem(NotificationLibrary notificationLibrary, CalendarLogic calendarLogic, LocalEvents localEvents, TimeLogic timeLogic)
        {
            _notificationLibrary = notificationLibrary;
            _calendarLogic = calendarLogic;
            _localEvents = localEvents;
            _timeLogic = timeLogic;
            _localEvents.OnNewMinute += CheckNotifications;
            _localEvents.OnNewNotificationCreated += AddNotification;
            _localEvents.OnMessangerButtonClick += HideNotifications;
            _notificationView = Object.FindObjectOfType<NotificationView>(includeInactive: true);
            _miniCalendarView = Object.FindObjectOfType<MiniCalendarView>(includeInactive: true);
            _miniMessageButton = Object.FindObjectOfType<MiniMessageButton>(includeInactive: true);
            
            _notificationPosition.Add(NotificationType.Message, _miniMessageButton.RectTransform.position);
            _notificationPosition.Add(NotificationType.Calendar, _miniCalendarView.RectTransform.position);
            
            _notifications = _notificationLibrary.GetNotifications();
            
            _notificationView.Init(_localEvents);
        }

        public void AddNotification(Notification notification)
        {
            Debug.Log($"Add notification: {notification.Title} at {notification.Day}/{notification.Month}/{notification.Year}");
            _notifications.Add(notification);
        }

        private void HideNotifications()
        {
            _notificationView.HideNotification();
        }

        private void CheckNotifications()
        {
            GameDate currentDate = _calendarLogic.GetCurrentDate();

            foreach (var notification in _notifications)
            {
                if (notification.Year == currentDate.Year &&
                    notification.Month == currentDate.Month &&
                    notification.Day == currentDate.Day &&
                    notification.Hour == _timeLogic.CurrentHour &&
                    notification.Minute == _timeLogic.CurrentMinute)
                {
                    _localEvents.TriggerNewNotification();
                    _notificationView.Notify(notification.Message, _notificationPosition[notification.Type]);
                    _timer = 0;
                    _isTimerRunning = true;
                }
            }
        }
        
        public void CleanUp()
        {
            _localEvents.OnNewMinute -= CheckNotifications;
            _localEvents.OnNewNotificationCreated -= AddNotification;
            _localEvents.OnMessangerButtonClick -= HideNotifications;
        }

        public void Execute(float deltatime)
        {
            if(!_isTimerRunning) return;
            _timer += deltatime;

            if (_timer >= _timeBeforeHideNotification)
            {
                _notificationView.HideNotification();
                _isTimerRunning = false;
            }
        }
    }
}