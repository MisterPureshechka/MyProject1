using System;
using Core;
using Scripts.Catalogues;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Sleep
{
    public class SleepLogic : ICleanUp
    {
        private GameStateMachine _gameStateMachine;
        private LocalEvents _localEvents;
        private ProgressDataAdapter _progressDataAdapter;
        private GameProgress _gameProgress;
        private CalendarLogic _calendarLogic;
        private TimeLogic _timeLogic;
        private AlarmClockCatalogue _alarmClockCatalogue;
        private int _sleepHours;
        private int _sleepMinutes;
        
        private bool _clockButtonsWired = false;

        public SleepLogic(GameStateMachine gameStateMachine, LocalEvents localEvents, ProgressDataAdapter progressDataAdapter, GameProgress gameProgress, CalendarLogic calendarLogic, TimeLogic timeLogic)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;
            _calendarLogic = calendarLogic;
            _timeLogic = timeLogic;

            _alarmClockCatalogue = Object.FindAnyObjectByType<AlarmClockCatalogue>(FindObjectsInactive.Include);

            _localEvents.OnHeroSleepState += ShowAlarmClock;
            
            SetAlarmClock();
        }

        private void ShowAlarmClock()
        {
            _sleepHours   = _timeLogic.CurrentHour;
            _sleepMinutes = _timeLogic.CurrentMinute;
            UpdateClockView();
            _localEvents.TriggerShowCatalogue(_alarmClockCatalogue);
        }

        private void SetAlarmClock()
        {
            _alarmClockCatalogue.ApplyButton.onClick.RemoveAllListeners();
            _alarmClockCatalogue.ApplyButton.onClick.AddListener(Sleep);

            _alarmClockCatalogue.CloseButton.onClick.RemoveAllListeners();
            _alarmClockCatalogue.CloseButton.onClick.AddListener(() =>
            {
                _localEvents.OnCatalogueHide(_alarmClockCatalogue);
            });

            WireClockButtonsOnce();
        }

        private void WireClockButtonsOnce()
        {
            if (_clockButtonsWired) return;        // защита от двойной регистрации
            _clockButtonsWired = true;

            // Часы: независимое +/-1 по модулю 24
            SetClockButton(
                _alarmClockCatalogue.HourButton,
                up:   () => { _sleepHours   = (_sleepHours   + 1 + 24) % 24; UpdateClockView(); },
                down: () => { _sleepHours   = (_sleepHours   - 1 + 24) % 24; UpdateClockView(); }
            );

            // Минуты: переход к ближайшей следующей/предыдущей четверти (без влияния на часы)
            SetClockButton(
                _alarmClockCatalogue.MinuteButton,
                up:   () => { _sleepMinutes = NextQuarter(_sleepMinutes);     UpdateClockView(); },
                down: () => { _sleepMinutes = PrevQuarter(_sleepMinutes);     UpdateClockView(); }
            );
        }

        private void Sleep()
        {
            int cur    = _timeLogic.CurrentHour * 60 + _timeLogic.CurrentMinute;
            int target = _sleepHours * 60 + _sleepMinutes;

            if (target <= cur) 
                _progressDataAdapter.TryUpdateValue(Consts.CurrentDayKey, 1);
            
            _localEvents.TriggerHideCatalogue(_alarmClockCatalogue);

            SaveTime(_sleepHours, _sleepMinutes);
            _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());
            _gameStateMachine.EnterState<LoadProgressState>();

        }
        
        private void SaveTime(int hour, int minute)
        {
            var meta = _progressDataAdapter.GetProgressData().Metadata;

            if (meta.TryGetValue(Consts.GameHourKey, out var hourData))   
                hourData.Value = hour;                                     

            if (meta.TryGetValue(Consts.GameMinuteKey, out var minuteData))
                minuteData.Value = minute;                                
        }


        private void SetClockButton(AlarmClockButton btn, Action up, Action down)
        {
            btn.UpButton.onClick.RemoveAllListeners();
            btn.UpButton.onClick.AddListener(() => up());

            btn.DownButton.onClick.RemoveAllListeners();
            btn.DownButton.onClick.AddListener(() => down());
        }

        private static int NextQuarter(int minute)
        {
            minute = Mathf.Clamp(minute, 0, 59);
            if (minute % 15 == 0)
                return (minute + 15) % 60;            
            return ((minute / 15) + 1) * 15 % 60;     
        }

        private static int PrevQuarter(int minute)
        {
            minute = Mathf.Clamp(minute, 0, 59);
            if (minute % 15 == 0)
                return (minute + 60 - 15) % 60;        
            return (minute / 15) * 15;                  
        }

        private void UpdateClockView()
        {
            _alarmClockCatalogue.ChangeClockHour(_sleepHours);
            _alarmClockCatalogue.ChangeClockMinute(_sleepMinutes);
        }

        private void SetSleepTime(int hours, int minutes)
        {
            _sleepHours = hours;
            _sleepMinutes = minutes;
        }

        public void CleanUp()
        {
            _localEvents.OnHeroSleepState -= Sleep;
        }
    }
}