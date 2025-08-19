using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Catalogues;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Messenger
{
    public class MessengerCatalogue : MonoBehaviour, ICatalogue
    {
        // [SerializeField] private SenderDataBase _senderDataBase;
        // [SerializeField] private SenderIconView _senderIconView;
        // [SerializeField] private RectTransform _senderIconsRect;

        [field: SerializeField] public Button CloseButton;
        
        [SerializeField] private RectTransform _messengerRect;
        [SerializeField] private TextMeshProUGUI _senderName;
        [SerializeField] private TextMeshProUGUI _senderText;
        [SerializeField] private TextMeshProUGUI _date;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _declineButton;
        [SerializeField] private GameObject _buttonsPanel;
        
        [Space]
        [SerializeField] Ease _bubbleEase;
        [SerializeField] Ease _letterEase;
        [SerializeField] Ease _hideEase;
        [Space]
        [SerializeField] private float _bubbleScaleDuration;
        [SerializeField] private float _scaleDuration;
        [SerializeField] private float _dropOffsetY;
        [SerializeField] private float _dropDuration;
        [SerializeField] private float _letterDelay;
        [SerializeField] private float _letterHideDuration;
        [Space]
        [SerializeField] private Vector3 _startScaleValue;
        [SerializeField] private Vector3 _scaleToValue;
        
        [field: SerializeField] public Button NextButton;
        [field: SerializeField] public Button PreviousButton;
        
        private Vector2 _startPosition = new Vector2(-60f, -45f);
        private Vector2 _hidePosition;
        private Vector2 _offset = new Vector2(0, -400f);
        
        private Sequence _sequence;
        private bool _isFisrtTimeShow = false;

        private void Start()
        {
            HideOnStart();
        }

        public void ShowNextButton(bool isActive)
        {
            NextButton.gameObject.SetActive(isActive);
        }

        public void ShowPreviousButton(bool isActive)
        {
            PreviousButton.gameObject.SetActive(isActive);
        }
        
        private void HideOnStart()
        {
            _messengerRect.gameObject.SetActive(false);
            _hidePosition = _startPosition + _offset;
            _messengerRect.localPosition = _hidePosition;
        }


        public void ShowMessage(IMessageSender sender)
        {
            _senderName.text = sender.Name;
            _senderText.text = sender.Message;

            _acceptButton.onClick.RemoveAllListeners();
            _declineButton.onClick.RemoveAllListeners();

            bool requiresDecision = sender.OnAccept != null;

            if (requiresDecision)
            {
                _acceptButton.onClick.AddListener(() =>
                {
                    sender.OnAccept?.Invoke();
                });
            }
            else
            {
                _buttonsPanel.SetActive(false);
            }

            var animator = new DOTweenTMPAnimator(_senderText);
            
            var delay = _letterDelay + _scaleDuration;
            
            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                animator.DOScaleChar(i, Vector3.zero, 0f);
            }

            var startDelay = _isFisrtTimeShow ? 0 : 0.6f;

            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                animator.DOOffsetChar(i, new Vector2(0, _dropOffsetY), 0f).SetDelay(i * delay + _bubbleScaleDuration/2 + startDelay);
                animator.DOScaleChar(i, Vector3.one, _scaleDuration).SetDelay(i * delay + _bubbleScaleDuration/2 + startDelay);
                animator.DOOffsetChar(i, Vector2.zero, _dropDuration)
                    .SetEase(_letterEase)
                    .SetDelay(i * delay + _bubbleScaleDuration/2 + startDelay);
            }

            _isFisrtTimeShow = false;
        }

        public void Show(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _messengerRect.gameObject.SetActive(true);
            _sequence.Append(_messengerRect.transform.DOLocalMove(_startPosition, 0.6f).SetEase(Ease.OutSine));
            _sequence.OnComplete(() =>
            {
                _isFisrtTimeShow = true;
                onComplete?.Invoke();
            });
        }

        public void Hide(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_messengerRect.transform.DOLocalMove(_hidePosition, 0.4f).SetEase(Ease.InSine));
            _sequence.OnComplete(() =>
            {
                _isFisrtTimeShow = true;
                _messengerRect.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        public bool IsVisible => _messengerRect && _messengerRect.gameObject.activeSelf;
    }
}
