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
        private readonly ProgressDataAdapter _progressDataAdapter;

        private GameDate _currentDate;


        public CalendarLogic(LocalEvents localEvents, MiniCalendarView miniCalendarView, CalendarCatalogue calendarCatalogue, ProgressDataAdapter progressDataAdapter)
        {
            _localEvents = localEvents;
            _miniCalendarView = miniCalendarView;
            _calendarCatalogue = calendarCatalogue;
            _progressDataAdapter = progressDataAdapter;

            _calendarCatalogue.Init(_localEvents);
            _miniCalendarView.Init(_localEvents, _calendarCatalogue);
            LoadOrCreateNewDay();

            _localEvents.OnNewDay += IncreaseDay;
        }

        private void LoadOrCreateNewDay()
        {
            var metadata = _progressDataAdapter.GetProgressData().Metadata;
            
            _currentDate.Day = (int)metadata.GetValue(Consts.CurrentDayKey);
            _currentDate.Month = (int)metadata.GetValue(Consts.CurrentMonthKey);
            _currentDate.Year = (int)metadata.GetValue(Consts.CurrentYearKey);

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
            if (_progressDataAdapter.GetProgressData().Metadata.TryGetValue(Consts.CurrentDayKey, out var dayData))
            {
                dayData.Value = date.Day;
            }

            if (_progressDataAdapter.GetProgressData().Metadata.TryGetValue(Consts.CurrentMonthKey, out var monthData))
            {
                monthData.Value = date.Month;
            }
            
            if (_progressDataAdapter.GetProgressData().Metadata.TryGetValue(Consts.CurrentYearKey, out var yearData))
            {
                yearData.Value = date.Year;
            }
        }

        public GameDate GetCurrentDate() => _currentDate;
        
        public void CleanUp()
        {
            _localEvents.OnNewDay -= IncreaseDay;
        }
    }
}