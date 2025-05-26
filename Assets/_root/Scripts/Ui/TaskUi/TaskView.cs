using System;
using System.Threading.Tasks;
using DG.Tweening;
using Scripts.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scripts.Ui.TaskUi
{
    public class TaskView : MonoBehaviour
    {
        private Sequence _fxTextSequence;
        private Sequence _imageSequence;
        private bool _isDestroyed;
        
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _progressInfo;
        [SerializeField] private TextMeshProUGUI _fxText;

        [SerializeField] private Image _spriteImage;
        [SerializeField] private Sprite[] _paperSprite;

        [SerializeField] private Color _gameDesignColor;
        [SerializeField] private Color _soundDesignColor;
        [SerializeField] private Color _programmingColor;
        [SerializeField] private Color _marketingColor;
        [SerializeField] private Color _artColor;


        [SerializeField] private float _offset = 0.01f;
        [SerializeField] private Vector3 _moveToValue;
        [SerializeField] private float _showDuration = 0.2f;
        [SerializeField] private float _hideDuration = 0.2f;

        private bool _isOnStart;
        private Vector3 _imageStartScale;

        private void Start()
        {
            GetStartSize();
        }

        private void GetStartSize()
        {
            _imageStartScale = _spriteImage.transform.localScale;
        }
        public void SetInfo(string titleText, float progressText)
        {
            //_titleText.text = titleText;
            //_progressInfo.text = progressText.ToString("0.0");
        }

        public void SetInfoIfDev(string titleText, float progressText, DevTaskType taskType)
        {
            //_titleText.text = titleText;
            _isOnStart = true;
            //_progressInfo.text = progressText.ToString("0.0");
            _spriteImage.sprite = _paperSprite[Random.Range(0, _paperSprite.Length)];
            _spriteImage.color = DevTypeToColor(taskType);
        }

        public void HideTask(Action onComplete = null)
        {
            if (_isOnStart || _isDestroyed)
            {
                onComplete?.Invoke();
                return;
            }
        
            _imageSequence?.Kill();

            if (_fxText == null || _fxText.transform == null)
            {
                onComplete?.Invoke();
                return;
            }
        
            _imageSequence = DOTween.Sequence();
            _imageSequence.Append(_spriteImage.transform.DOScale(Vector3.zero, _hideDuration)
                .OnComplete(() => onComplete?.Invoke()));
        }
        
        public async Task HideTaskAsync()
        {
            if (_isOnStart || _isDestroyed)
                return;

            _imageSequence?.Kill();

            if (_fxText == null || _fxText.transform == null)
                return;

            _imageSequence = DOTween.Sequence();

            var tween = _spriteImage.transform.DOScale(Vector3.zero, _hideDuration);
            _imageSequence.Append(tween);

            await tween.AsyncWaitForCompletion();
        }
        
        public void ShowTask()
        {
            _imageSequence?.Kill();
            _imageSequence = DOTween.Sequence();
            _spriteImage.transform.localScale = Vector3.zero;
    
            _imageSequence.Append(_spriteImage.transform.DOScale(Vector3.one, _showDuration))
                .OnComplete(() => _isOnStart = false); 
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
            _fxTextSequence?.Kill();
            _imageSequence?.Kill();
        }

        public void UpdateProgress(float progress, float value)
        {
            if (_isDestroyed) return;
        
            _isOnStart = false;
            //_progressInfo.text = progress.ToString("0.0");
        }

        public void AnimateTextFx(float value)
        {
            if(_isOnStart || _isDestroyed) return;
        
            _fxTextSequence?.Kill();
        
            if (_fxText == null || _fxText.transform == null) return;
        
            _fxTextSequence = DOTween.Sequence();
        
            var offset = Random.Range(-_offset, _offset);
            _fxText.text = value.ToString("0.0");
        
            if (this == null || transform == null) return;
        
            _fxText.transform.position = transform.position + new Vector3(offset, offset, 0);
        
            _fxTextSequence.Append(_fxText.DOFade(1, 0));
            _fxTextSequence.Append(_fxText.transform.DOMove(
                _fxText.transform.position + _moveToValue, 
                0.5f).SetEase(Ease.OutSine));
            _fxTextSequence.Join(_fxText.DOFade(0, 0.5f).SetEase(Ease.OutSine));
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
    }
}