using System;
using System.Collections.Generic;
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
        [SerializeField] private Image _fxImage;
        [SerializeField] private Sprite[] _paperSprite;
        [SerializeField] private Sprite[] _taskSprites;
        [SerializeField] private Sprite[] _fxSprites;
        [SerializeField] private AnimationCurve[] _fxCurves;

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
        private SprintType _currentSprintType;
        private bool _isAppearing;

        private void Start()
        {
            GetStartSize();
        }

        private void GetStartSize()
        {
            _imageStartScale = _spriteImage.transform.localScale;
        }
        public void SetInfo(string titleText, float progressText, SprintType sprintType)
        {
            _currentSprintType = sprintType;
            
            switch (sprintType)
            {
                case SprintType.Chill:
                    _spriteImage.sprite = _taskSprites[0];
                    break;
                case SprintType.Eat:
                    _spriteImage.sprite = _taskSprites[1];
                    break;
                case SprintType.Read:
                    _spriteImage.sprite = _taskSprites[2];
                    break;
                case SprintType.Coffee:
                    _spriteImage.sprite = _taskSprites[3];
                    break;
                case SprintType.Toilet:
                    _spriteImage.sprite = _taskSprites[4];
                    break;
                case SprintType.Shower:
                    _spriteImage.sprite = _taskSprites[5];
                    break;
                case SprintType.CleanPc:
                    _spriteImage.sprite = _taskSprites[5];
                    break;
                case SprintType.Play:
                    _spriteImage.sprite = _taskSprites[6];
                    break;
            }
            
            _spriteImage.color = Color.white;
        }

        public void SetInfoIfDev(string titleText, float progressText, DevTaskType taskType)
        {
            _isOnStart = true;
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

            if (_fxText == null)
                return;

            _imageSequence = DOTween.Sequence();

            var tween = _spriteImage.transform.DOScale(Vector3.zero, _hideDuration);
            _imageSequence.Append(tween);

            await tween.AsyncWaitForCompletion();
        }
        
        public void ShowTask()
        {
            _isOnStart = true;
            _isAppearing = true;
            _imageSequence?.Kill();
            _imageSequence = DOTween.Sequence();
            _spriteImage.transform.localScale = Vector3.zero;
    
            _imageSequence.Append(_spriteImage.transform.DOScale(Vector3.one, _showDuration)
                .OnComplete(() =>
                {
                    _isOnStart = false;
                    _isAppearing = false;
                })); 
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
            _fxTextSequence?.Kill();
            _imageSequence?.Kill();
        }

        public void UpdateProgress(float progress, float value)
        {
            if (_isDestroyed || _isAppearing) return;
        
            _isOnStart = false;
        }

        public void AnimateTextFx(float value, float duration)
        {
            if(_isOnStart || _isDestroyed || _isAppearing) return;
            
            if (_fxText == null || _fxText.transform == null) return;
        
            _fxTextSequence?.Kill();
            _fxTextSequence = DOTween.Sequence();
            
            if (_currentSprintType == SprintType.Shower || _currentSprintType == SprintType.CleanPc)
            {
                var offset = Random.Range(-_offset, _offset);
        
                if (this == null || transform == null) return;
                
                SetBubbleSprite();
                    
                _fxImage.rectTransform.localPosition = new Vector3(offset, offset, 0);
                _fxTextSequence.Append(_fxImage.rectTransform.DOLocalMoveY(
                    _fxImage.rectTransform.localPosition.y + _moveToValue.y, 
                    duration * 0.8f).SetEase(Ease.InSine));
                _fxTextSequence.Join(_fxImage.rectTransform.DOLocalMoveX(
                    10f,
                    duration * 0.8f).SetEase(_fxCurves[Random.Range(0, _fxCurves.Length)]));
                _fxTextSequence.Append(_fxImage.transform.DOScale(
                    _fxImage.transform.localScale * 1.5f, 
                    duration * 0.2f).SetEase(Ease.InSine).OnComplete(() => _fxImage.gameObject.SetActive(false)));
            }
            else if (_currentSprintType == SprintType.Toilet)
            {
                var offset = Random.Range(-_offset, _offset);
                _fxText.text = value.ToString("0.0");
        
                if (this == null || transform == null) return;
        
                _fxText.rectTransform.localPosition = new Vector3(offset, offset, 0);
        
                _fxTextSequence.Append(_fxText.DOFade(1, 0));
                _fxTextSequence.Append(_fxText.rectTransform.DOLocalMove(
                    _fxText.rectTransform.localPosition - _moveToValue, 
                    duration).SetEase(Ease.InSine));
                _fxTextSequence.Join(_fxText.DOFade(0, 0.5f).SetEase(Ease.InSine));
            }
            else
            {
                var offset = Random.Range(-_offset, _offset);
                _fxText.text = value.ToString("0.0");
                _fxText.transform.localScale = Vector3.zero;
                if (this == null || transform == null) return;
        
                _fxText.rectTransform.localPosition = new Vector3(offset, offset, 0);
        
                _fxTextSequence.Append(_fxText.DOFade(1, 0));
                _fxTextSequence.Append(_fxText.DOScale(1, duration * 0.1f));
                _fxTextSequence.Append(_fxText.rectTransform.DOLocalMove(
                    _fxText.rectTransform.localPosition + _moveToValue, 
                    duration).SetEase(Ease.InSine));
                _fxTextSequence.Join(_fxText.DOFade(0, 0.5f).SetEase(Ease.InSine));
            }
            
            _fxTextSequence.Join(_spriteImage.transform.DOShakeRotation(0.1f, 45, 100, 50).SetEase(Ease.OutSine).OnStart(() =>
            {
                _spriteImage.transform.DOScale(Vector3.one * 1.1f, 0.1f)
                    .OnComplete(() =>
                    {
                        _spriteImage.transform.localScale = Vector3.one;
                    });
            }));
            
        }

        private void SetBubbleSprite()
        {
            _fxImage.gameObject.SetActive(true);
            float scaleOffset = Random.Range(1.1f, 1.5f);
            
            _fxImage.sprite = _fxSprites[0];
            _fxImage.transform.localScale = Vector3.one * scaleOffset;
            
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

        public void StopFx()
        {
            if (_fxImage)
            {
                _fxImage.gameObject.SetActive(false);
                _fxImage.transform.localRotation = Quaternion.identity;
            }
           
            _fxTextSequence?.Kill();
            _fxText.gameObject.SetActive(false);
        }
    }
}