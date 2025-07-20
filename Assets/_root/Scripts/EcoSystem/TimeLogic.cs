using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class TimeLogic : IExecute, ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly TimeView _timeView;
        private readonly LocalEvents _localEvents;

        float _currentTime;
        
        private float _timeMultiplier;
        private float _realSecondsPerDay = 2400f;

        public TimeLogic(ProgressDataAdapter progressDataAdapter, TimeView timeView, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _timeView = timeView;
            _localEvents = localEvents;
            _currentTime = _progressDataAdapter.GetProgressData().Metadata.GetValue("GameTime");
            
            SetTimeSpeed();
            _localEvents.OnActiveSprint += SpeedUpTime;
            _localEvents.OnSprintExit += SpeedDownTime;
        }
        
        private void SetTimeSpeed()
        {
            _timeMultiplier = 24f / _realSecondsPerDay;
        }
        
        private void SpeedUpTime()
        {
            _realSecondsPerDay = 24f;
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

            _timeView.UpdateTimeText(_currentTime);
            
            if (_progressDataAdapter.GetProgressData().Metadata.TryGetValue("GameTime", out var metadata))
            {
                metadata.Value = _currentTime;
            }
        }

        private void CalculateTime(float deltatime)
        {
            _currentTime += deltatime * _timeMultiplier;
            if (_currentTime >= 24f)
            {
                _localEvents.TriggerNewDay();
                _currentTime = 0;
            }
            
            float dayValue = CalculateDayValue(_currentTime);
            _localEvents.TriggerOnDayTimeChange(dayValue);
            float normalizedDayTime = GetNormalizedDayTime(_currentTime);
            _localEvents.TriggerNormalizeDayTimeChange(normalizedDayTime);
            float normalizeNightValue = GetNormalizedNightTime(_currentTime);
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
            // Вечер: 20 → 24 (растёт от 0 до 1)
            if (currentTime >= 20f && currentTime <= 24f)
                return Mathf.InverseLerp(20f, 24f, currentTime);

            if (currentTime >= 0f && currentTime < 12f)
                return Mathf.InverseLerp(0f, 12f, currentTime) * -1 + 1; 

            return 0f;
        }

        public void CleanUp()
        {
            _localEvents.OnActiveSprint -= SpeedUpTime;
            _localEvents.OnSprintExit -= SpeedDownTime;
        }
    }
}