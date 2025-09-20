using System;
using System.Collections.Generic;
using _root.Notification;
using DG.Tweening;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = Unity.Mathematics.Random;

namespace Scripts.EcoSystem.Calendar
{
    public class CalendarCatalogue : MonoBehaviour, ICatalogue
    {
        [SerializeField] private RectTransform _calendarPanel;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private DayUI _dayUiPrefab;
        [SerializeField] private DayEvent _dayEventPrefab;
        [SerializeField] private TextMeshProUGUI _monthName;
        [SerializeField] private TextMeshProUGUI _dayNames;

        [SerializeField] private Vector3 _showPosition = new Vector3(0, -75f, 0);
        [SerializeField] private Vector3 _hidePosition = new Vector3(0, -400f, 0);
        [SerializeField] private Color[] _colors;
        private int _currentColorId;
        
        [SerializeField] private Transform _daysHolder;

        private Vector3 _startScale;
        private int _currentMonth = 9;
        private int _currentYear = 2025;

        private List<DayUI> _days = new();
        private List<MonthUI> _months = new();
        private List<CalendarEvent> _allEvents;
        private List<List<CalendarEvent>> _permanentEvents;
        
        private bool _isVisible;
        private LocalEvents _localEvents;

        public void Init(LocalEvents localEvents, List<CalendarEvent> calendarEvents, List<List<CalendarEvent>> permanentEvents)
        {
            _localEvents = localEvents;
            
            _localEvents.OnCalendarNoteAdded += CreatePermanentEvents;
            _localEvents.OnCalendarEventCreated += CreateEvent;
            
            _allEvents = calendarEvents;
            _permanentEvents = permanentEvents;
            
            _leftButton.onClick.AddListener(OnLeftButtonClicked);
            _rightButton.onClick.AddListener(OnRightButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            
            _calendarPanel.localPosition = _hidePosition;
            
            //CreateTempEvent();
            SetDays();
            ShowMonth(_currentMonth, _currentYear);
        }

        private void Awake()
        {
            _startScale = transform.localScale;
        }
        
        private void OnCloseButtonClicked()
        {
            _localEvents.TriggerHideCatalogue(this);
        }

        private void CreatePermanentEvents(string message, int day)
        {
            var permanentEvent = new List<CalendarEvent>();

            var hour = 9;
            var minute = 0;

            for (int i = 0; i < 12; i++)
            {
                int rawMonth = _currentMonth + i;
                int year  = _currentYear + (rawMonth - 1) / 12;
                int month = ((rawMonth - 1) % 12) + 1;

                int maxDay = CalendarExtentions.GetDaysInMonth(month, year);
                int clampedDay = Mathf.Clamp(day, 1, maxDay);

                var calendarEvent = new CalendarEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Day = clampedDay,
                    Month = month,
                    Year = year,
                    Message = message
                };
                permanentEvent.Add(calendarEvent);
                
                var notification = new Notification(Guid.NewGuid().ToString(), NotificationType.Calendar, message, message, year, month, clampedDay, hour, minute);
                
                CreateEventNotification(calendarEvent);
                _localEvents.TriggerNewNotificationCreated(notification);
            }

            _permanentEvents.Add(permanentEvent);
        }

        private void CreateEvent(CalendarEvent eventToCreate)
        {
            _allEvents.Add(eventToCreate);
            CreateEventNotification(eventToCreate);
        }

        private void CreateEventNotification(CalendarEvent calendarEvent)
        {
            var notification = new Notification(
                Guid.NewGuid().ToString(), NotificationType.Calendar, calendarEvent.Message, calendarEvent.Message, 
                calendarEvent.Year, calendarEvent.Month, calendarEvent.Day, 10, 0);
                
            _localEvents.TriggerNewNotificationCreated(notification);
        }

        private void SetDays()
        {
            _dayNames.text = "";
            
            for (int i = 1; i <= 7; i++)
            {
                string space = i == 7 ? "" : " ";
                _dayNames.text += CalendarExtentions.GetMiniDayName(i) + space;
            }

            ClearDays();

            for (int i = 0; i < 42; i++) 
            {
                var dayInstance = Instantiate(_dayUiPrefab, _daysHolder).GetComponent<DayUI>();
                _days.Add(dayInstance);
            }
        }

        private void ClearDays()
        {
            foreach (var dayInstance in _days)
            {
                Object.Destroy(dayInstance.gameObject);
            }
            
            _days.Clear();
        }
        
        private void ShowMonth(int month, int year)
        {
            _currentMonth = month;
            _monthName.text = CalendarExtentions.GetMonthName(month);
    
            int daysInMonth = CalendarExtentions.GetDaysInMonth(month, year);
            int dayOffset = CalendarExtentions.GetDayOfWeek(1, month, year); // 0=Mon

            for (int i = 0; i < _days.Count; i++)
            {
                var dayUI = _days[i];
                dayUI.ClearEvents();

                if (i < dayOffset || i >= dayOffset + daysInMonth)
                {
                    dayUI.SetDayNumber(0);
                }
                else
                {
                    int dayNumber = i - dayOffset + 1;
                    dayUI.gameObject.SetActive(true);
                    dayUI.SetDayNumber(dayNumber);

                    foreach (var ev in _allEvents)
                    {
                        if (ev.Day == dayNumber && ev.Month == month && ev.Year == year)
                        {
                            var eventInstance = Instantiate(_dayEventPrefab, dayUI.transform);
                            eventInstance.UpdateEventInfo(ev.Message, _colors[_currentColorId]);
                            dayUI.AddEvent(eventInstance);
                            _currentColorId = (_currentColorId + 1) % _colors.Length;
                        }
                    }
                    
                    foreach (var permanentEvent in _permanentEvents)
                    {
                        foreach (var ev in permanentEvent)
                        {
                            if (ev.Day == dayNumber && ev.Month == month && ev.Year == year)
                            {
                                var eventInstance = Instantiate(_dayEventPrefab, dayUI.transform);
                                eventInstance.UpdateEventInfo(ev.Message, _colors[_currentColorId]);
                                dayUI.AddEvent(eventInstance);
                                _currentColorId = (_currentColorId + 1) % _colors.Length;
                            }
                        }
                    }
                }
            }
        }

        private void OnRightButtonClicked()
        {
            AnimateCalendar(false);
            
            _currentMonth++;
            if (_currentMonth > 12)
            {
                _currentMonth = 1;
                _currentYear++;
            }

            ShowMonth(_currentMonth, _currentYear);
        }

        private void OnLeftButtonClicked()
        {
            AnimateCalendar(true);
            
            _currentMonth--;
            if (_currentMonth < 1)
            {
                _currentMonth = 12;
                _currentYear--;
            }

            ShowMonth(_currentMonth, _currentYear);
        }

        private void AnimateCalendar(bool isLeft)
        {
            transform.DORotate(new Vector3(0, 0, isLeft ? 2.0f : -2.0f), 0.1f)
                .SetEase(Ease.OutSine)
                .OnComplete(() => transform.localRotation = Quaternion.identity);
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveAllListeners();
            _leftButton?.onClick.RemoveAllListeners();
            _rightButton?.onClick.RemoveAllListeners();
            _localEvents.OnCalendarNoteAdded -= CreatePermanentEvents;
            _localEvents.OnCalendarEventCreated -= CreateEvent;
        }

        public void Show(Action onComplete = null)
        {
            ShowMonth(_currentMonth, _currentYear);
            _isVisible = true;
            gameObject.SetActive(true);
            _calendarPanel.DOLocalMoveY(_showPosition.y, 0.4f).SetEase(Ease.OutSine)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Hide(Action onComplete = null)
        {
            _calendarPanel.DOLocalMoveY(_hidePosition.y, 0.4f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    _isVisible = false;
                    gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }

        public bool IsVisible => _isVisible;
    }
}