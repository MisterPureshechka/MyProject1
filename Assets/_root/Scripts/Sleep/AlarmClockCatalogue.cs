using System;
using DG.Tweening;
using Scripts.Catalogues;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Sleep
{
    public class AlarmClockCatalogue : MonoBehaviour, ICatalogue
    {
        [field: SerializeField] public Button CloseButton;
        [field: SerializeField] public Button ApplyButton;

        [field: SerializeField] public AlarmClockButton HourButton;
        [field: SerializeField] public AlarmClockButton MinuteButton;

        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _clockRect;
        [SerializeField] private TextMeshProUGUI _hourText;
        [SerializeField] private TextMeshProUGUI _hourMinuteText;
        
        private Sequence _sequence;
        private bool _isVisible;
        
        private Vector2 _startPosition = new Vector2(0f, -60f);
        private Vector2 _hidePosition;
        private Vector2 _offset = new Vector2(0, -400f);
        

        private void Start()
        {
            _hidePosition = _startPosition + _offset;
            HideAllOnStart();
        }

        public void Hide(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _isVisible = true;
            _root.gameObject.SetActive(true);
            _sequence.Append(_root.transform.DOLocalMove(_hidePosition, 0.4f).SetEase(Ease.InSine));
            _sequence.OnComplete(() => onComplete?.Invoke());
        }

        private void HideAllOnStart()
        {
            _root.gameObject.SetActive(false);
            _root.localPosition = _hidePosition;
        }

        public void Show(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _root.gameObject.SetActive(true);
            
            _sequence.Append(_root.transform.DOLocalMove(_startPosition, 0.6f).SetEase(Ease.OutSine));
            _sequence.OnComplete(() =>
            {
                _isVisible = false;
                onComplete?.Invoke();
            });
        }
        
        public void Shake(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _clockRect.gameObject.SetActive(true);
            _sequence.Append(_clockRect.transform.DOShakePosition(0.2f, new Vector3(1f,1f,0), 50, 90f).SetEase(Ease.OutSine));
            _sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        
        public bool IsVisible { get; }

        public void ChangeClockHour(int valueToChange)
        {
            Shake();
            var zero = IsOneDigit(valueToChange) ? "0" : "";
            _hourText.text = zero + valueToChange;
        }

        public void ChangeClockMinute(int valueToChange)
        {
            Shake();
            var zero = IsOneDigit(valueToChange) ? "0" : "";
            _hourMinuteText.text = zero + valueToChange;
        }
        
        private bool IsOneDigit(int value)
        {
            return value < 10;
        }
    }
}