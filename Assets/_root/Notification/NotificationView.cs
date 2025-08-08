using DG.Tweening;
using Scripts.GlobalStateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Notification
{
    public class NotificationView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _bubbleImage;
        [SerializeField] private Button _button;
        [Space]
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

        private LocalEvents _localEvents;

        private Sequence _sequence;

        private void Awake()
        {
            _button.onClick.AddListener(() => HideNotification());
            _text.raycastTarget = false;
            
            var mat = _text.fontSharedMaterial;
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 100f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -100f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 50f);
        }

        public void Notify(string message)
        {
            gameObject.SetActive(true);
            _localEvents.TriggerNewNotification();
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            Vector2 startSize = _startScaleValue;
            
            _bubbleImage.gameObject.SetActive(true);
            var width = _bubbleImage.rectTransform.sizeDelta;
            width = startSize;
            _bubbleImage.rectTransform.sizeDelta = width;

            float targetWidth = _scaleToValue.x; 

            _sequence.Append(
                DOTween.To(
                    () => _bubbleImage.rectTransform.sizeDelta,
                    x => _bubbleImage.rectTransform.sizeDelta = x,
                    new Vector2(targetWidth, startSize.y),
                    _bubbleScaleDuration
                ).SetEase(_bubbleEase)
            );
            
            _text.text = message;
            
            var animator = new DOTweenTMPAnimator(_text);
            
            var delay = _letterDelay + _scaleDuration;
            
            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                animator.DOScaleChar(i, Vector3.zero, 0f);
            }

            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                animator.DOOffsetChar(i, new Vector2(0, _dropOffsetY), 0f).SetDelay(i * delay + _bubbleScaleDuration/2);
                animator.DOScaleChar(i, Vector3.one, _scaleDuration).SetDelay(i * delay + _bubbleScaleDuration/2);
                animator.DOOffsetChar(i, Vector2.zero, _dropDuration)
                    .SetEase(_letterEase)
                    .SetDelay(i * delay + _bubbleScaleDuration/2);
            }
        }

        public void HideNotification()
        { 
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            var animator = new DOTweenTMPAnimator(_text);
            
            var delay = _letterDelay + _letterHideDuration;
            var delayMultiplier = 0;
            
            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                animator.DOOffsetChar(i, new Vector2(0, _dropOffsetY), _dropDuration).SetDelay(i * delay);
                animator.DOScaleChar(i, Vector3.zero, 0).SetDelay(i * delay + _dropDuration);
                //delayMultiplier++;
            }
            
            //_sequence.SetDelay(animator.textInfo.characterCount/2 * delay);
            var hideDuration = animator.textInfo.characterCount * (delay + _letterHideDuration);
            
            float targetWidth = _startScaleValue.x; 
            
            _sequence.Append(
                DOTween.To(
                    () => _bubbleImage.rectTransform.sizeDelta,
                    x => _bubbleImage.rectTransform.sizeDelta = x,
                    new Vector2(targetWidth, _startScaleValue.y),
                    hideDuration
                ).SetEase(_hideEase).OnComplete(() =>
                {
                    _bubbleImage.gameObject.SetActive(false);
                })
            );
            
            
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Notify("You pressed space button!");
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            
        }
    }
}