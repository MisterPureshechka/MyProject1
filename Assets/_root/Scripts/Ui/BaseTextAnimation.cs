using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Scripts.Ui
{
    public class BaseTextAnimation : MonoBehaviour
    {
        [SerializeField] private TextAnimationConfig _animationConfig;
        [SerializeField] private RectTransform _textToAnimate;
        private Vector3 _defaultScale;
        private Vector3 _defaultPosition;
        private Vector3 _defaultRotation;
        private Sequence _seq;

        private void OnEnable()
        {
            // _defaultScale = _textToAnimate.localScale;
            // _defaultPosition = _textToAnimate.position;
            // _defaultRotation = _textToAnimate.rotation.eulerAngles;
        }
        public void AnimateText()
        {
            var cfg = _animationConfig;

            _seq?.Kill();
            _seq = DOTween.Sequence().SetUpdate(true); 

            _textToAnimate.DOPunchRotation(new Vector3(0, 0, cfg.RotatateToValue), cfg.Duration, cfg.Vibrato, 0.5f)
                .OnComplete(() => { ResetText(); });
            
            _seq.Join(_textToAnimate.DOScale(Vector3.one * cfg.ScaleToValue, cfg.Duration * 0.5f));
            _seq.Append(_textToAnimate.DOScale(Vector3.one, cfg.Duration * 0.5f));

            _seq.Play();
        }

        private void ResetText()
        {
            var rotation = _textToAnimate.localRotation.eulerAngles;
            rotation.z = 0;
        }

        private void Reset()
        {
            // _textToAnimate.localScale = _defaultScale;
            // _textToAnimate.position = _defaultPosition;
            // _textToAnimate.eulerAngles = _defaultRotation;
        }
    }
}