using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _root.Notification
{
    public class SpeechBubbleView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _bubble;
        
        [SerializeField] private TextBubbleConfig _config;

        private DOTweenTMPAnimator _animator;
        private Coroutine _typingCoroutine;
        private Coroutine _autoHideCoroutine;
        private Tween _shakeTween;

        public void ShowBubble(string text)
        {
            gameObject.SetActive(true);
            _text.text = text;
            _text.alpha = 0f;
            _text.ForceMeshUpdate();

            if (_typingCoroutine != null) { StopCoroutine(_typingCoroutine); _typingCoroutine = null; }
            if (_autoHideCoroutine != null) { StopCoroutine(_autoHideCoroutine); _autoHideCoroutine = null; }
            _shakeTween?.Kill();

            _bubble.localScale = Vector3.one * _config.BubbleStartSize;
            _bubble.DOScale(1f, _config.GrowDuration).SetEase(Ease.OutBack);

            _shakeTween = _bubble.DOShakeAnchorPos(
                    duration: _config.GrowDuration,
                    strength: new Vector2(2f, 2f),
                    vibrato: _config.BubbleVibrato,
                    randomness: 90,
                    fadeOut: false);

            _typingCoroutine = StartCoroutine(TypeTextRoutine());
            _autoHideCoroutine = StartCoroutine(AutoHideRoutine());
        }

        private IEnumerator TypeTextRoutine()
        {
            _text.ForceMeshUpdate();                
            _animator = new DOTweenTMPAnimator(_text);

           
            for (int i = 0; i < _animator.textInfo.characterCount; i++)
            {
                if (_animator.textInfo.characterInfo[i].isVisible)
                    _animator.DOFadeChar(i, _config.StartAlpha, 0f);
            }

            yield return null; 

            for (int i = 0; i < _animator.textInfo.characterCount; i++)
            {
                if (!_animator.textInfo.characterInfo[i].isVisible)
                    continue;
                
                _animator.DOFadeChar(i, 0f, 0f); 

                _animator
                    .DOOffsetChar(i, new Vector3(0f, _config.CharMoveY, 0f), _config.CharAnimDuration) 
                    .From()
                    .SetEase(Ease.OutQuad);

                _animator.DOFadeChar(i, 1f, _config.CharAnimDuration); 

                float delay = _config.CharacterDelay + Random.Range(0f, _config.RandomDelay);
                yield return new WaitForSeconds(delay);
            }
        }
        
        private IEnumerator AutoHideRoutine()
        {
            yield return new WaitForSeconds(_config.AutoHideDelay);
            HideBubble();
        }
        
        public void SetPosition(Vector2 screenPosition, Camera camera)
        {
            screenPosition += new Vector2(0, _config.YOffset);

            var parent = _root.parent as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, camera, out var localPoint))
            {
                _root.anchoredPosition = ClampTo(parent, localPoint);
            }
        }
        
        private Vector2 ClampTo(RectTransform parent, Vector2 pos)
        {
            var halfSize = _bubble.rect.size * 0.5f;
            var parentHalf = parent.rect.size * 0.5f;

            pos.x = Mathf.Clamp(pos.x, -parentHalf.x + halfSize.x, parentHalf.x - halfSize.x);
            pos.y = Mathf.Clamp(pos.y, -parentHalf.y + halfSize.y, parentHalf.y - halfSize.y);
            return pos;
        }

        public void HideBubble()
        {
            if (_typingCoroutine != null) { StopCoroutine(_typingCoroutine); _typingCoroutine = null; }
            if (_autoHideCoroutine != null) { StopCoroutine(_autoHideCoroutine); _autoHideCoroutine = null; }
            _shakeTween?.Kill();

            // плавное исчезновение
            _bubble.DOScale(0.2f, _config.HideDuration).SetEase(Ease.InOutQuad);
            _text.DOFade(0f, _config.HideDuration)
                .OnComplete(() => gameObject.SetActive(false));
        }
        
    }
}