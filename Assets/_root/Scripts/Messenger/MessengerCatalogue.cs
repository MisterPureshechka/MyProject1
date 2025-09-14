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
        [field: SerializeField] public Button CloseButton;
        
        [SerializeField] private RectTransform _messengerRect;
        [SerializeField] private TextMeshProUGUI _senderName;
        [SerializeField] private TextMeshProUGUI _senderText;
        [SerializeField] private TextMeshProUGUI _date;
        [SerializeField] private MessengerAcceptButton _acceptButton;
        [SerializeField] private MessengerAcceptButton _declineButton;
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


        public void ShowMessage(
            IMessageSender sender,
            MessangerButtonState state,
            Action onAcceptPressed = null,
            Action onDeclinePressed = null)
        {
            _senderName.text = sender.Name;
            _senderText.text = sender.Message;

            _acceptButton.Button.onClick.RemoveAllListeners();
            _declineButton.Button.onClick.RemoveAllListeners();

            bool requiresDecision = sender.OnAccept != null;
            _buttonsPanel.SetActive(requiresDecision);

            if (requiresDecision)
            {
                LoadButtonState(state);

                _acceptButton.Button.onClick.AddListener(() =>
                {
                    if (state == MessangerButtonState.None) {
                        sender.OnAccept?.Invoke();
                    }
                    _acceptButton.SwitchToggle();
                    _declineButton.DisableButton();
                    Shake();
                    onAcceptPressed?.Invoke();
                });

                _declineButton.Button.onClick.AddListener(() =>
                {
                    _declineButton.SwitchToggle();
                    _acceptButton.DisableButton();
                    onDeclinePressed?.Invoke();
                    Shake();
                });
            }
            else
            {
                _buttonsPanel.SetActive(false);
            }

            var animator = new DOTweenTMPAnimator(_senderText);
            var delay = _letterDelay + _scaleDuration;

            for (int i = 0; i < animator.textInfo.characterCount; i++)
                animator.DOScaleChar(i, Vector3.zero, 0f);

            var startDelay = _isFisrtTimeShow ? 0 : 0.6f;

            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                animator.DOOffsetChar(i, new Vector2(0, _dropOffsetY), 0f).SetDelay(i * delay + _bubbleScaleDuration/2 + startDelay);
                animator.DOScaleChar(i, Vector3.one, _scaleDuration).SetDelay(i * delay + _bubbleScaleDuration/2 + startDelay);
                animator.DOOffsetChar(i, Vector2.zero, _dropDuration).SetEase(_letterEase).SetDelay(i * delay + _bubbleScaleDuration/2 + startDelay);
            }

            _isFisrtTimeShow = false;
        }


        private void LoadButtonState(MessangerButtonState state)
        {
            Debug.LogWarning("Loading button state " + state);
            switch (state)
            {
                case MessangerButtonState.Accepted:
                    _acceptButton.SwitchToggleImmediate(true);
                    _declineButton.DisableButton();
                    break;
                case MessangerButtonState.Declined:
                    _declineButton.SwitchToggleImmediate(true);
                    _acceptButton.DisableButton();
                    break;
                case MessangerButtonState.None:
                    _declineButton.ResetButton();
                    _acceptButton.ResetButton();
                    break;
                default:
                    _declineButton.ResetButton();
                    _acceptButton.ResetButton();
                    break;
            }
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
        
        public void Shake(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _messengerRect.gameObject.SetActive(true);
            _sequence.Append(_messengerRect.transform.DOShakePosition(0.2f, new Vector3(1f,1f,0), 50, 90f).SetEase(Ease.OutSine));
            _sequence.OnComplete(() =>
            {
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
