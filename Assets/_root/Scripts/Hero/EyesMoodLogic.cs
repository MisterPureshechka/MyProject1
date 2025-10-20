using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Hero
{
    public class EyesMoodLogic : ICleanUp
    {
        private const string Mood = "Mood";
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;

        private float _moodValue;
        private MoodState _currentMoodState;

        public EyesMoodLogic(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            
            _progressDataAdapter.OnStatUpdated += UpdateMood;
            UpdateMood();
            
           
        }

        private void UpdateMood()
        {
            var moodData = _progressDataAdapter.GetMetadata(Mood);
            
            _moodValue = moodData.Value;
            float maxMood = moodData.MaxValue;
            float normalized = Mathf.Clamp01(_moodValue / maxMood);

            MoodState newMoodState;
            
            if (normalized < 0.66f)
            {
                if (normalized < 0.33f)
                {
                    newMoodState = MoodState.Sad; 
                }
                else
                {
                    newMoodState = MoodState.Normal;
                } 
            }
            else
            {
                newMoodState = MoodState.Happy;
            }
                
            if (newMoodState != _currentMoodState)
            {
                _currentMoodState = newMoodState;
                _localEvents.TriggerEyesMoodChanged(_currentMoodState);
                Debug.Log($"[HeroMoodLogic] Mood changed: {_currentMoodState}");
            }
        }

        public void CleanUp()
        {
            _progressDataAdapter.OnStatUpdated -= UpdateMood;
        }
    }
}