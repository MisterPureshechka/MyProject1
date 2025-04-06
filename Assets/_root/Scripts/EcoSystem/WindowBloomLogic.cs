using Core;
using DG.Tweening;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class WindowBloomLogic : ICleanUp
    {
        private BloomView[] _bloomViews;
        
        private float _currentBloomIntensity;
        
        private Sequence _sequence;

        public WindowBloomLogic()
        {
            _currentBloomIntensity = 0.6f;
            GetAllBlooms();
            ResetBlooms(_currentBloomIntensity);
        }

        private void ResetBlooms(float bloomIntensity)
        {
            foreach (var bloomView in _bloomViews)
            {
                bloomView.SpriteRenderer.DOFade(bloomIntensity, 0);
            }
        }

        private void GetAllBlooms()
        {
            _bloomViews = Object.FindObjectsByType<BloomView>(FindObjectsSortMode.None);

            foreach (var bloomView in _bloomViews)
            {
                bloomView.OnHeroEnter += EnterHeroCallback;
                bloomView.OnHeroExit += ExitHeroCallback;
            }
        }

        private void EnterHeroCallback(BloomView bloomView)
        {
            var spriteRenderer = bloomView.SpriteRenderer;
            spriteRenderer.DOFade(0.1f, 0.5f);
            Debug.Log($"Enter Hero Callback: {bloomView}");
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
        }
    }
}