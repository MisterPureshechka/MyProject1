using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Hero
{
    public class ShowerStateLogic : ICleanUp
    {
        private const string Shower = "Shower";
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;
        private readonly HeroView _heroView;

        private float _showerValue;
        private CleanState _currentCleanState;

        public ShowerStateLogic(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents, HeroView heroView)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            _heroView = heroView;

            _progressDataAdapter.OnStatUpdated += UpdateCleanState;
            UpdateCleanState();
        }
        
        private void UpdateCleanState()
        {
            var moodData = _progressDataAdapter.GetMetadata(Shower);
            _showerValue = moodData.Value;
            float maxMood = moodData.MaxValue;
            float normalized = Mathf.Clamp01(_showerValue / maxMood);

            CleanState newCleanState;
            
            float emissionRate = 0f;

            if (normalized < 0.15f)
            {
                
                float t = 1f - Mathf.Clamp01(normalized / 0.15f);
                emissionRate = Mathf.Lerp(0f, 10f, t);
                newCleanState = CleanState.SmellsLikeShit;
            }
            else if (normalized < 0.5f)
                newCleanState = CleanState.Dirty;
            else
                newCleanState = CleanState.Clean;

            if (newCleanState == CleanState.SmellsLikeShit)
            {
                newCleanState = CleanState.Dirty;
            }
            
            _heroView.EmitFx(emissionRate);
            
            if (newCleanState != _currentCleanState)
            {
                _currentCleanState = newCleanState;
                _localEvents.TriggerCleanStateChange(_currentCleanState);
                Debug.Log($"[HeroMoodLogic] Mood changed: {_currentCleanState}");
            }
        }

        public void CleanUp()
        {
            _progressDataAdapter.OnStatUpdated -= UpdateCleanState;
        }
    }
}