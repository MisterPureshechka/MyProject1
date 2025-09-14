using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class BaseButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] public Button Button;
        [SerializeField] private ButtonAnimationData _config;

        private Sequence _seq;
        private RectTransform _rect;

        private bool _isActive = true;

        public void SetButtonActive(bool isActive)
        {
            _isActive = isActive;
        }

        private void Awake()
        {
            _rect = (Button != null ? Button.transform : transform) as RectTransform;
            if (_rect == null)
                Debug.LogError("[BaseButtonAnimation] RectTransform not found");
            if (_config == null)
                Debug.LogWarning("[BaseButtonAnimation] Config is null. Using defaults may cause odd behavior.");
        }

        public void OnPointerEnter(PointerEventData data)
        {
            KillSeq(complete: true);
            AnimateDefaultEnter();
            OnEnter(data);
            _rect.SetAsLastSibling();
        }

        public void OnPointerExit(PointerEventData data)
        {
            KillSeq(complete: true);
            OnExit(data);  
        }

        public void OnPointerClick(PointerEventData data)
        {
            OnClick(data);
            AnimateOnClick();
        }

        protected virtual void OnEnter(PointerEventData data) { }
        protected virtual void OnExit(PointerEventData data) { }
        protected virtual void OnClick(PointerEventData data) { }

        private void AnimateDefaultEnter()
        {
            if (_rect == null || _config == null || !_isActive) return;

            _seq = DOTween.Sequence().SetLink(gameObject);

            _seq.Append(_rect.DOShakeAnchorPos(
                duration: _config.ShakeDuration,
                strength: _config.ShakeStrengthPos,
                vibrato: _config.Vibrato,
                randomness: 0,
                snapping: false,
                fadeOut: _config.FadeOut
            ));

            if (_config.UsePunchScale)
            {
                _seq.Join(_rect.DOPunchScale(Vector3.one * _config.PunchAmount, _config.ShakeDuration, 6, 0.5f));
            }
            else
            {
                _seq.Join(_rect.DOScale(_config.OnEnterScaleValue, _config.OnEnterScaleDuration).SetEase(Ease.OutQuad));
            }
        }
        
        private void AnimateOnClick()
        {
            if (_rect == null || _config == null) return;

            if (_isActive)
            {
                _seq = DOTween.Sequence().SetLink(gameObject);

                _seq.Append(_rect.DOShakeAnchorPos(
                    duration: _config.ShakeDuration,
                    strength: _config.ShakeStrengthPos,
                    vibrato: _config.Vibrato,
                    randomness: _config.Randomness,
                    snapping: false,
                    fadeOut: _config.FadeOut
                ).SetEase(_config.OnClickScaleEase));

                _seq.Join(_rect.DOShakeRotation(
                    duration: _config.ShakeDuration,
                    strength: _config.ShakeStrengthRot,
                    vibrato: _config.Vibrato,
                    randomness: _config.Randomness,
                    fadeOut: _config.FadeOut
                ));

                if (_config.UsePunchScale)
                {
                    _seq.Join(_rect.DOPunchScale(Vector3.one * _config.PunchAmount, _config.ShakeDuration, 6, 0.5f));
                }
                else
                {
                    _seq.Join(_rect.DOScale(_config.OnClickScaleToValue, _config.OnClickDuration).SetEase(Ease.OutQuad));
                    _seq.Append(_rect.DOScale(1, _config.OnClickDuration).SetEase(Ease.OutQuad));
                } 
            }
            else
            {
                _seq = DOTween.Sequence().SetLink(gameObject);

                _seq.Append(_rect.DOShakeAnchorPos(
                    duration: _config.ShakeDuration,
                    strength: _config.ShakeStrengthPos,
                    vibrato: _config.Vibrato,
                    randomness: _config.Randomness,
                    snapping: false,
                    fadeOut: _config.FadeOut
                ).SetEase(_config.OnClickScaleEase));
            }
        }

        private void KillSeq(bool complete)
        {
            if (_seq == null) return;
            _seq.Kill(complete);       
            _seq = null;
            _rect.localScale = Vector3.one; 
        }

        private void OnDisable() => KillSeq(true);
    }
}
