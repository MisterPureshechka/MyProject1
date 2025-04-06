using Core;
using DG.Tweening;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Rooms
{
    public class InteractiveObjectAnimator : ICleanUp
    {
        private readonly InteractiveObjectConfig _config;
        private readonly IInteractiveObject _io;
        private SpriteRenderer _spriteRenderer;
        
        private Sequence _sequence;

        public InteractiveObjectAnimator(InteractiveObjectConfig config, IInteractiveObject io)
        {
            _config = config;
            _io = io;

            _spriteRenderer = _io.spriteRenderer;

            _io.OnCursorEnter += AnimateCursorEnter;
            _io.OnCursorExit += AnimateCursorExit;
        }

        public void Execute(float deltatime)
        {
        }

        private void AnimateCursorEnter()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            

            _sequence.Append(_spriteRenderer.DOFade(0.4f, _config.AnimationSpeed));
        }
        
        private void AnimateCursorExit()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _sequence.Append(_spriteRenderer.DOFade(0, _config.AnimationSpeed));
        }

        public void CleanUp()
        {
            _io.OnCursorEnter -= AnimateCursorEnter;
            _io.OnCursorExit -= AnimateCursorExit;
        }
    }
}