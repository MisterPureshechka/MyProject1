using System;
using DG.Tweening;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.EcoSystem.Calendar
{
    public class MiniCalendarView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI _currentMonth;
        [SerializeField] private TextMeshProUGUI _currentDay;
        [SerializeField] private Image _miniCalendarIcon;
        [SerializeField] private Button _openCalendarButton;
        [Space] 
        [SerializeField] private AnimationCurve _endToZeroCurve;
        [SerializeField] private Vector3 _shakeStrength = new Vector3(1.0f, 1.0f, 0f);
        [SerializeField] private Vector3 _shakeRotationValue = new Vector3(0f, 0f, 25f);
        [SerializeField] private float _shakeDuration = .2f;
        
        [SerializeField] private RectTransform _rect;
        public RectTransform RectTransform => _rect;
        
        private LocalEvents _localEvents;
        private ICatalogue _calendar;
        private Sequence _sequence;
        private Vector3 _iconStartPosition;
        private Vector3 _textStartPosition;


        private void Awake()
        {
            _openCalendarButton.onClick.AddListener(ButtonListener);
        }

        private void Start()
        {
            _iconStartPosition = _miniCalendarIcon.transform.position;
            _textStartPosition = _currentDay.transform.position;
            
            _openCalendarButton.gameObject.SetActive(true);
            _openCalendarButton.targetGraphic.raycastTarget = true;
            _miniCalendarIcon.raycastTarget = false;
            _currentDay.raycastTarget = false;
            _currentMonth.raycastTarget = false;
        }

        private void ButtonListener()
        {
            _localEvents.TriggerShowCatalogue(_calendar);
            AnimateButtonOnEnter(1.1f);
        }

        public void Init(LocalEvents localEvents, ICatalogue calendar)
        {
            _localEvents = localEvents;
            _calendar = calendar;
        }

        public void UpdateMiniCalendar(int month, int day)
        {
            UpdateDay(day);
            UpdateMonth(month);
        }
        private void UpdateMonth(int month)
        {
            _currentMonth.text = CalendarExtentions.GetMiniMonthName(month);
        }

        private void UpdateDay(int day)
        {
            var dayOfWeek = CalendarExtentions.GetMiniDayName(day);
            _currentDay.text = $"{day} {dayOfWeek}";
        }

        private void OnDestroy()
        {
            _openCalendarButton.onClick.RemoveAllListeners();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateButtonOnEnter();
        }

        public void AnimateButtonOnEnter(float strength = 1f)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            var shakeStrength = _shakeStrength * strength;
            var endValue = 1.1f * strength;
            var vector3 = new Vector3(0, 0, 5f) * strength;

            _sequence.Append(_miniCalendarIcon.transform.DOShakePosition(
                duration: _shakeDuration,
                strength: vector3,
                vibrato: 10,
                randomness: 90,
                snapping: false,
                fadeOut: false
            ).SetEase(_endToZeroCurve));

            if (strength != 1f)
            {
                _sequence.Join(_miniCalendarIcon.transform.DOShakeRotation(
                    duration: _shakeDuration/2,
                    strength: _shakeRotationValue,
                    vibrato: 90,
                    randomness: 50f,
                    fadeOut: false
                ).SetEase(_endToZeroCurve)
                    .OnComplete(() =>
                    {
                        _miniCalendarIcon.transform.localRotation = Quaternion.Euler(0,0,0);
                    }));
            }
            
            
            _sequence.Join(_currentDay.transform.DOShakePosition(_shakeDuration, shakeStrength, 50, 200f));
            _sequence.Join(gameObject.transform.DOScale(endValue, _shakeDuration/2).SetEase(_endToZeroCurve));


            //
            // _sequence.Append(_miniCalendarIcon.transform.DOMove(_iconStartPosition, _shakeDuration).SetEase(Ease.OutSine));
            // _sequence.Join(_currentDay.transform.DOMove(_textStartPosition, _shakeDuration).SetEase(Ease.OutSine));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //AnimateButtonOnExit();
        }

        private void AnimateButtonOnExit()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(gameObject.transform.DOScale(1, 0.1f).SetEase(Ease.OutBack));
            _sequence.Join(_miniCalendarIcon.transform.DOMove(_iconStartPosition, _shakeDuration).SetEase(Ease.InOutSine));
            _sequence.Join(_currentDay.transform.DOMove(_textStartPosition, _shakeDuration).SetEase(Ease.InOutSine));
        }
    }
}