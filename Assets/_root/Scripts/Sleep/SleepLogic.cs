using System;
using Core;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Scripts.Sleep
{
    public class SleepLogic : ICleanUp
    {
        private readonly AlarmClockCatalogue _alarmClockCatalogue;

        private bool _clockButtonsWired;
        private readonly GameProgress _gameProgress;
        private readonly GameStateMachine _gameStateMachine;
        private readonly LocalEvents _localEvents;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private int _sleepHours;
        private int _sleepMinutes;
        private readonly TimeLogic _timeLogic;

        public SleepLogic(GameStateMachine gameStateMachine, LocalEvents localEvents,
            ProgressDataAdapter progressDataAdapter, GameProgress gameProgress,
            TimeLogic timeLogic)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;
            _timeLogic = timeLogic;

            _alarmClockCatalogue = Object.FindAnyObjectByType<AlarmClockCatalogue>(FindObjectsInactive.Include);

            _localEvents.OnHeroSleepState += ShowAlarmClock;

            SetAlarmClock();
        }

        public void CleanUp()
        {
            _localEvents.OnHeroSleepState -= ShowAlarmClock;
        }

        private void ShowAlarmClock()
        {
            _sleepHours = _timeLogic.CurrentHour;
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
            if (_clockButtonsWired) return;
            _clockButtonsWired = true;

            SetClockButton(
                _alarmClockCatalogue.HourButton,
                () =>
                {
                    _sleepHours = (_sleepHours + 1 + 24) % 24;
                    UpdateClockView();
                },
                () =>
                {
                    _sleepHours = (_sleepHours - 1 + 24) % 24;
                    UpdateClockView();
                }
            );

            SetClockButton(
                _alarmClockCatalogue.MinuteButton,
                () =>
                {
                    _sleepMinutes = NextQuarter(_sleepMinutes);
                    UpdateClockView();
                },
                () =>
                {
                    _sleepMinutes = PrevQuarter(_sleepMinutes);
                    UpdateClockView();
                }
            );
        }

        private void Sleep()
        {
            var cur = _timeLogic.CurrentHour * 60 + _timeLogic.CurrentMinute;
            var target = _sleepHours * 60 + _sleepMinutes;

            var durationMinutes = target - cur;
            if (durationMinutes <= 0)
            {
                durationMinutes += 24 * 60;
                _progressDataAdapter.TryUpdateValue(Consts.CurrentDayKey, 1);
            }

            CreateChangeStatValue(durationMinutes);

            _localEvents.TriggerHideCatalogue(_alarmClockCatalogue);
            SaveTime(_sleepHours, _sleepMinutes);

            _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());
            _gameStateMachine.EnterState<HomeState>();
        }

        private void CreateChangeStatValue(int durationMinutes)
        {
            var hoursSlept = durationMinutes / 60f;

            var energyPerHour = Random.Range(8f, 12f);
            var energyDelta = hoursSlept * energyPerHour;
            _progressDataAdapter.TryUpdateValue(Consts.Energy, energyDelta);

            float moodDelta;
            if (hoursSlept < 5f)
                moodDelta = -Random.Range(10f, 25f);
            else if (hoursSlept < 8f)
                moodDelta = Random.Range(5f, 15f);
            else if (hoursSlept <= 10f)
                moodDelta = Random.Range(1f, 8f);
            else
                moodDelta = -Random.Range(5f, 12f); 

            _progressDataAdapter.TryUpdateValue(Consts.Mood, moodDelta);

            var hungerPerHour = Random.Range(4.5f, 6.0f);
            var foodDelta = -(hoursSlept * hungerPerHour + Random.Range(0f, 5f));
            _progressDataAdapter.TryUpdateValue(Consts.Food, foodDelta);
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
            return (minute / 15 + 1) * 15 % 60;
        }

        private static int PrevQuarter(int minute)
        {
            minute = Mathf.Clamp(minute, 0, 59);
            if (minute % 15 == 0)
                return (minute + 60 - 15) % 60;
            return minute / 15 * 15;
        }

        private void UpdateClockView()
        {
            _alarmClockCatalogue.ChangeClockHour(_sleepHours);
            _alarmClockCatalogue.ChangeClockMinute(_sleepMinutes);
        }
    }
}