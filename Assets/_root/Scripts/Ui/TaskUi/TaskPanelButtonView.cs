using DG.Tweening;
using Scripts.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class TaskPanelButtonView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        [field: SerializeField] public Button TaskPannelButton;
        [SerializeField] private Image _shadow;
        
        [SerializeField] private Sprite[] _paperSprite;
        
        [SerializeField] private Color _gameDesignColor;
        [SerializeField] private Color _soundDesignColor;
        [SerializeField] private Color _programmingColor;
        [SerializeField] private Color _marketingColor;
        [SerializeField] private Color _artColor;
        
        [SerializeField] private Vector3 _shakeValue = new Vector3(0f,0f,15f);
        [SerializeField] private float _hoverScaleAmount = 1.7f;
        [SerializeField] private float _hoverDuration = 0.2f;
        [SerializeField] private Ease _hoverEase = Ease.OutBack;

        private Tween _currentTween;
        
        private Vector3 _originalScale;
        private Vector2 _originalSize;
        private LayoutElement _layoutElement;
        private RectTransform _rectTransform;
        private float _shadowOpacity;
        

        private void Awake()
        {
            _originalScale = transform.localScale;
            _shadowOpacity = _shadow.color.a;
            _image.alphaHitTestMinimumThreshold = 0.5f;
            _image.useSpriteMesh = true;
            
            SetupHoverEvents();
        }

        public void UpdateInfo(string info, DevTaskType type)
        {
            _text.text = info;
            _image.sprite = _paperSprite[Random.Range(0, _paperSprite.Length)];
            _image.color = DevTypeToColor(type);
        }
        public void UpdateInfo(string info)
        {
            _text.text = info;
        }

        private void SetupHoverEvents()
        {
            var trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => OnPointerEnter());
            trigger.triggers.Add(enterEntry);

            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => OnPointerExit());
            trigger.triggers.Add(exitEntry);

            var clickEntry = new EventTrigger.Entry();
            clickEntry.callback.AddListener(_ => OnPointerClick());
            trigger.triggers.Add(clickEntry);
        }

        private void OnPointerEnter()
        {
            transform.SetAsLastSibling();
            _currentTween?.Kill();
            
            _currentTween = transform.DOScale(_originalScale * _hoverScaleAmount, _hoverDuration)
                .SetEase(_hoverEase);
            _currentTween = _shadow.DOFade(0, _hoverDuration)
                .SetEase(_hoverEase);
        }

        private void OnPointerExit()
        {
            _currentTween?.Kill();
            
            _currentTween = transform.DOScale(_originalScale, _hoverDuration)
                .SetEase(_hoverEase);
            _currentTween = _shadow.DOFade(_shadowOpacity, _hoverDuration)
                .SetEase(_hoverEase);
        }

        private void OnPointerClick()
        {
            _currentTween?.Kill();
            transform.eulerAngles = Vector3.zero;
            _currentTween = transform.DOShakeRotation(0.1f, _shakeValue).SetEase(Ease.OutSine);
        }
        
        private Color DevTypeToColor(DevTaskType devType)
        {
            switch (devType)
            {
                case DevTaskType.Art:
                    return _artColor;
                case DevTaskType.Marketing:
                    return _marketingColor;
                case DevTaskType.Programming :
                    return _programmingColor;
                case DevTaskType.SoundDesign:
                    return _soundDesignColor;
                case DevTaskType.GameDesign:
                    return _gameDesignColor;
                default:
                    return Color.white;
            }
        }

        public void AddToRoot(Transform newRoot)
        {
            transform.SetParent(newRoot);
        }
    }
}