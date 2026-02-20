using System.Collections.Generic;
using _root.Notification;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.EcoSystem.Calendar
{
    public class CalendarLogic : ICleanUp
    {
        private LocalEvents _localEvents;
        private readonly MiniCalendarView _miniCalendarView;
        private readonly CalendarCatalogue _calendarCatalogue;
        private readonly ProgressDataAdapterOLD _progressDataAdapterOld;
        
        private List<CalendarEvent> _allEvents = new();
        private List<List<CalendarEvent>> _permanentEvents = new();

        private GameDate _currentDate;


        public CalendarLogic(LocalEvents localEvents, MiniCalendarView miniCalendarView, CalendarCatalogue calendarCatalogue, ProgressDataAdapterOLD progressDataAdapterOld)
        {
            _localEvents = localEvents;
            _miniCalendarView = miniCalendarView;
            _calendarCatalogue = calendarCatalogue;
            _progressDataAdapterOld = progressDataAdapterOld;

            _calendarCatalogue.Init(_localEvents, _allEvents, _permanentEvents);
            _miniCalendarView.Init(_localEvents, _calendarCatalogue);
            LoadOrCreateNewDay();

            _localEvents.OnNewDay += IncreaseDay;
            _localEvents.OnNewNotificatiom += AnimateIcon;
        }

        private void AnimateIcon()
        {
            _miniCalendarView.AnimateButtonOnEnter();
        }

        private void LoadOrCreateNewDay()
        {

            if (_currentDate.Day == 0)
            {
                _currentDate = new GameDate
                {
                    Day = 7,
                    Month = 9,
                    Year = 2025
                };
            }

            _miniCalendarView.UpdateMiniCalendar(_currentDate.Month, _currentDate.Day);
        }

        private void IncreaseDay()
        {
            _currentDate.Day++;

            int daysInCurrentMonth = CalendarExtentions.DaysInMonths[_currentDate.Month - 1];

            if (_currentDate.Day > daysInCurrentMonth)
            {
                _currentDate.Day = 1;
                _currentDate.Month++;

                if (_currentDate.Month > 12)
                {
                    _currentDate.Month = 1;
                    _currentDate.Year++;
                }
            }
            
            SaveDate(_currentDate);

            _miniCalendarView.UpdateMiniCalendar(_currentDate.Month, _currentDate.Day);
        }

        private void SaveDate(GameDate date)
        {
            
        }

        public bool IsWeekend()
        {
            var date = _currentDate;
            return CalendarExtentions.IsWeekend(date.Day, date.Month, date.Year);
        }

        public GameDate GetCurrentDate() => _currentDate;
        
        public void CleanUp()
        {
            _localEvents.OnNewDay -= IncreaseDay;
            _localEvents.OnNewNotificatiom -= AnimateIcon;
        }
    }
}