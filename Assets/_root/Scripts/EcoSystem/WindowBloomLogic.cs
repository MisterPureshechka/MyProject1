using Core;
using DG.Tweening;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class WindowBloomLogic : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private BloomView[] _bloomViews;
        
        private float _currentBloomIntensity;
        
        private Sequence _sequence;

        public WindowBloomLogic(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            GetAllBlooms();
            ResetBlooms(_currentBloomIntensity);
            
            _localEvents.OnNormilizeDayTimeChange += UpdateBloomByTime;
        }

        private void ResetBlooms(float bloomIntensity)
        {
            foreach (var bloomView in _bloomViews)
            {
                bloomView.SpriteRenderer.DOFade(bloomIntensity, 0);
            }
        }
        
        private void UpdateBloomByTime(float value)
        {
            float peak = Mathf.Sin(value * Mathf.PI); 
            _currentBloomIntensity = Mathf.Lerp(0f, 0.6f, peak);

            foreach (var bloomView in _bloomViews)
            {
                if (bloomView.IsHeroInside || bloomView == null) continue;

                bloomView.SpriteRenderer.DOFade(_currentBloomIntensity, 0.5f);
            }
        }

        private void GetAllBlooms()
        {
            _bloomViews = Object.FindObjectsByType<BloomView>(FindObjectsSortMode.None);

            foreach (var bloomView in _bloomViews)
            {
                if (bloomView == null) continue;
                
                bloomView.OnHeroEnter += EnterHeroCallback;
                bloomView.OnHeroExit += ExitHeroCallback;
            }
        }

        private void EnterHeroCallback(BloomView bloomView)
        {
            var spriteRenderer = bloomView.SpriteRenderer;
            spriteRenderer.DOFade(0.1f, 0.5f);
        }
        
        private void ExitHeroCallback(BloomView bloomView)
        {
            var spriteRenderer = bloomView.SpriteRenderer;
            spriteRenderer.DOFade(_currentBloomIntensity, 0.5f);
        }

        public void CleanUp()
        {
            foreach (var bloomView in _bloomViews)
            {
                bloomView.OnHeroEnter -= EnterHeroCallback;
                bloomView.OnHeroExit -= ExitHeroCallback;
            }
            
            _localEvents.OnDayTimeChange -= UpdateBloomByTime;
        }
    }
}