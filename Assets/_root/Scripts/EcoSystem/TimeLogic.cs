using System;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class TimeLogic : IExecute, ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly TimeView _timeView;
        private readonly LocalEvents _localEvents;

        float _currentHour;
        float _currentMinute;
        
        private float _timeMultiplier;
        private float _realSecondsPerDay = 2400f;
        private int _lastMinute = -1;
        private int _lastHour = -1;

        public TimeLogic(ProgressDataAdapter progressDataAdapter, TimeView timeView, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _timeView = timeView;
            _localEvents = localEvents;


            _currentHour = 60f;

            _lastHour   = Mathf.FloorToInt(_currentHour);
            _lastMinute = Mathf.FloorToInt((_currentHour % 1f) * 60);
            
            Debug.Log($"Current hour intimeLogic: {_currentHour}:{_currentMinute}");
            
            _lastHour = Mathf.FloorToInt(_currentHour);  
            
            SetTimeSpeed();
            _localEvents.OnActiveSprint += SpeedUpTime;
            _localEvents.OnSprintExit += SpeedDownTime;
            _localEvents.OnNewHour += SaveHour;
            _localEvents.OnNewMinute += SaveMinute;
        }

        private void SaveHour()
        {
        }

        private void SaveMinute()
        {
        }

        private void SetTimeSpeed()
        {
            _timeMultiplier = 24f / _realSecondsPerDay;
        }
        
        private void SpeedUpTime()
        {
            _realSecondsPerDay = 240f;
            SetTimeSpeed();
        }

        private void SpeedDownTime()
        {
            _realSecondsPerDay = 2400f;
            SetTimeSpeed();
        }

        public void Execute(float deltatime)
        {
            CalculateTime(deltatime);

            _timeView.UpdateTimeText(_currentHour);
        }

        private void CalculateTime(float deltatime)
        {
            _currentHour += deltatime * _timeMultiplier;
            
            if (_currentHour >= 24f)
            {
                _localEvents.TriggerNewDay();
                _currentHour = 0;
            }
            
            int hourNow = CurrentHour;              // FloorToInt(_currentHour)
            if (hourNow != _lastHour)
            {
                _lastHour = hourNow;
                _localEvents.TriggerNewHour();
            }
            
            if (CurrentMinute != _lastMinute)
            {
                _lastMinute = CurrentMinute;
                _localEvents.TriggerNewMinute();
            }
            
            float dayValue = CalculateDayValue(_currentHour);
            _localEvents.TriggerOnDayTimeChange(dayValue);
            float normalizedDayTime = GetNormalizedDayTime(_currentHour);
            _localEvents.TriggerNormalizeDayTimeChange(normalizedDayTime);
            float normalizeNightValue = GetNormalizedNightTime(_currentHour);
            _localEvents.TriggerNormalizeNightTimeChange(normalizeNightValue);
        }
        
        private float CalculateDayValue(float time)
        {
            if (time >= 6f && time < 12f)
                return Mathf.InverseLerp(6f, 12f, time); 

            if (time >= 12f && time < 18f)
                return 1f;

            if (time >= 18f && time < 24f)
                return Mathf.InverseLerp(24f, 18f, time); 

            return 0f; 
        }
        
        public float GetNormalizedDayTime(float currentTime)
        {
            var startOfPeak = 11f;
            var endOfPeak = 18f;
            
            if (currentTime >= startOfPeak && currentTime < endOfPeak)
                return Mathf.InverseLerp(startOfPeak, endOfPeak, currentTime); 
            return 0f;
        }
        
        public float GetNormalizedNightTime(float currentTime)
        {
            if (currentTime >= 20f && currentTime <= 24f)
                return Mathf.InverseLerp(20f, 24f, currentTime);

            if (currentTime >= 0f && currentTime < 12f)
                return Mathf.InverseLerp(0f, 12f, currentTime) * -1 + 1; 

            return 0f;
        }
        
        public int CurrentHour => Mathf.FloorToInt(_currentHour);
        public int CurrentMinute => Mathf.FloorToInt((_currentHour % 1f) * 60);

        public void CleanUp()
        {
            _localEvents.OnActiveSprint -= SpeedUpTime;
            _localEvents.OnSprintExit   -= SpeedDownTime;
            _localEvents.OnNewHour      -= SaveHour;
            _localEvents.OnNewMinute    -= SaveMinute;
        }
    }
}