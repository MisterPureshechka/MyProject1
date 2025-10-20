using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Hero
{
    public class BodyStateLogic : ICleanUp
    {
        private const string Energy = "Energy";
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;

        private float _energyValue;
        private MoodState _currentMoodState;

        public BodyStateLogic(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            
            _progressDataAdapter.OnStatUpdated += UpdateMood;
            UpdateMood();
        }
        
        private void UpdateMood()
        {
            var energyData = _progressDataAdapter.GetMetadata(Energy);
            _energyValue = energyData.Value;
            
            float maxMood = energyData.MaxValue;
            float normalized = Mathf.Clamp01(_energyValue / maxMood);

            MoodState newMoodState;

            if (normalized < 0.66f)
            {
                if (normalized < 0.33f)
                {
                    newMoodState = MoodState.Sad;
                    _localEvents.TriggerLowEnergy(true);
                }
                else
                {
                    newMoodState = MoodState.Normal;
                    _localEvents.TriggerLowEnergy(false);
                }
            }  
            else
                newMoodState = MoodState.Happy;

            if (newMoodState != _currentMoodState)
            {
                _currentMoodState = newMoodState;
                _localEvents.TriggerBodyMoodChanged(_currentMoodState);
                Debug.Log($"[HeroMoodLogic] Mood changed: {_currentMoodState}");
            }
        }

        public void CleanUp()
        {
            _progressDataAdapter.OnStatUpdated -= UpdateMood;
        }
    }
}