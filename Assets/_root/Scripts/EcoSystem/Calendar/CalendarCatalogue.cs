using System;
using System.Collections.Generic;
using _root.Notification;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

namespace Scripts.EcoSystem.Calendar
{
    public class CalendarCatalogue : MonoBehaviour
    {
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private DayUI _dayUiPrefab;
        [SerializeField] private DayEvent _dayEventPrefab;
        [SerializeField] private TextMeshProUGUI _monthName;
        [SerializeField] private TextMeshProUGUI _dayNames;

        [SerializeField] private Vector3 _showPosition = new Vector3(0, -75f, 0);
        [SerializeField] private Vector3 _hidePosition = new Vector3(0, -1000f, 0);
        [SerializeField] private Color[] _colors;
        private int _currentColorId;
        
        [SerializeField] private Transform _daysHolder;

        private Vector3 _startScale;
        private int _currentMonth = 9;
        private int _currentYear = 2025;

        private List<DayUI> _days = new();
        private List<MonthUI> _months = new();
        private List<CalendarEvent> _allEvents = new();
        

        private void Awake()
        {
            _startScale = transform.localScale;
        }
        

        private void Start()
        {
            _leftButton.onClick.AddListener(OnLeftButtonClicked);
            _rightButton.onClick.AddListener(OnRightButtonClicked);
            
            transform.localPosition = _hidePosition;
            
            CreateTempEvent();
            SetDays();
            ShowMonth(_currentMonth, _currentYear);
        }

        private void CreateTempEvent()
        {
            _allEvents.Add(new CalendarEvent
            {
                Day = 15,
                Message = "Meeting",
                Month = 9,
                Year = 2025
            });
            _allEvents.Add(new CalendarEvent
            {
                Day = 15,
                Message = "NetWorking",
                Month = 9,
                Year = 2025
            });
            
            _allEvents.Add(new CalendarEvent
            {
                Day = 17,
                Message = "Conference",
                Month = 9,
                Year = 2025
            });
            
            _allEvents.Add(new CalendarEvent
            {
                Day = 18,
                Message = "Meet up with family",
                Month = 9,
                Year = 2025
            });
            
            _allEvents.Add(new CalendarEvent
            {
                Day = 5,
                Message = "Meet up with family",
                Month = 10,
                Year = 2025
            });
            _allEvents.Add(new CalendarEvent
            {
                Day = 5,
                Message = "Chill",
                Month = 10,
                Year = 2025
            });
            
            _allEvents.Add(new CalendarEvent
            {
                Day = 4,
                Message = "Meet up with family",
                Month = 9,
                Year = 2025
            });
        }

        private void SetDays()
        {
            for (int i = 1; i <= 7; i++)
            {
                string space = i == 7 ? "" : " ";
                _dayNames.text += CalendarExtentions.GetMiniDayName(i) + space;
            }
            
            _days.Clear();

            for (int i = 0; i < 42; i++) 
            {
                var dayInstance = Instantiate(_dayUiPrefab, _daysHolder).GetComponent<DayUI>();
                _days.Add(dayInstance);
            }
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
            // transform.DOScale(_startScale * 1.1f, 0.1f).SetEase(Ease.OutBack)
            //     .OnComplete(() => transform.DOScale(_startScale, 0.1f));
            
            transform.DORotate(new Vector3(0, 0, isLeft ? 2.0f : -2.0f), 0.1f)
                .SetEase(Ease.OutSine)
                .OnComplete(() => transform.localRotation = Quaternion.identity);
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveAllListeners();
            _leftButton?.onClick.RemoveAllListeners();
            _rightButton?.onClick.RemoveAllListeners();
        }

        public void ShowCatalogue()
        {
            transform.DOMoveY(_showPosition.y, 0.1f).SetEase(Ease.OutSine);
        }

        public void HideCatalogue()
        {
            transform.DOMoveY(_hidePosition.y, 0.1f).SetEase(Ease.InSine);
        }
    }
}