using System;
using Scripts.GlobalStateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.EcoSystem.Calendar
{
    public class MiniCalendarView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentMonth;
        [SerializeField] private TextMeshProUGUI _currentDay;
        [SerializeField] private Image _miniCalendarIcon;
        [SerializeField] private Button _openCalendarButton;
        
        private LocalEvents _localEvents;

        private void Awake()
        {
            _openCalendarButton.onClick.AddListener(ButtonListener);
        }

        private void ButtonListener()
        {
            Debug.Log("Button send");
            _localEvents.TriggerMiniCalendarButtonOpen();
        }

        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }

        public void UpdateMiniCalendar(int month, int day)
        {
            UpdateDay(day);
            UpdateMonth(month);
        }
        private void UpdateMonth(int month)
        {
            _currentMonth.text = CalendarExtentions.GetMiniMonthName(month);
        }

        private void UpdateDay(int day)
        {
            var dayOfWeek = CalendarExtentions.GetMiniDayName(day);
            _currentDay.text = $"{day} {dayOfWeek}";
        }

        private void OnDestroy()
        {
            _openCalendarButton.onClick.RemoveAllListeners();
        }
    }
}