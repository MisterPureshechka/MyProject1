using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Scripts.GlobalStateMachine;
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

        [SerializeField] private Image[] _extraSprites;
        [SerializeField] private TextMeshProUGUI _extraSpriteText;
        
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
        private LocalEvents _localEvents;
        
        private Sequence _extraSpriteSequence;

        private void Start()
        {
            GetStartSize();
        }
        
        private Sequence _extraHideSequence;

        private Sequence BuildHideExtrasSequence()
        {
            _extraHideSequence?.Kill();
            _extraHideSequence = DOTween.Sequence();

            var active = new List<Image>();
            foreach (var img in _extraSprites)
                if (img != null && img.gameObject.activeSelf)
                    active.Add(img);

            if (active.Count == 0)
            {
                return _extraHideSequence;
            }

            float step = _showDuration * 2;

            int idx = 0;
            for (int i = active.Count - 1; i >= 0; i--)
            {
                var img = active[i];
                img.transform.DOKill();

                _extraHideSequence.Insert(
                    step * idx++,
                    img.transform.DOScale(Vector3.zero, step)
                        .OnComplete(() => img.gameObject.SetActive(false))
                );
            }

            if (_extraSpriteText != null && _extraSpriteText.gameObject.activeSelf)
            {
                _extraHideSequence.Insert(0f,
                    _extraSpriteText.transform
                        .DOScale(Vector3.zero, step * 0.8f)
                        .OnComplete(() =>
                        {
                            _extraSpriteText.gameObject.SetActive(false);
                            _extraSpriteText.transform.localScale = Vector3.one;
                        })
                );
            }

            return _extraHideSequence;
        }

        private void GetStartSize()
        {
            _imageStartScale = _spriteImage.transform.localScale;
        }
        public void SetInfo(string titleText, float progressText, SprintType sprintType, LocalEvents localEvents)
        {
            if(sprintType != SprintType.Dev) HideAllExtras();
            
            _currentSprintType = sprintType;
            _localEvents = localEvents;
            
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

        public void SetInfoIfDev(string titleText, float progressText, DevTaskType taskType, LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _isOnStart = true;
            _spriteImage.sprite = _paperSprite[Random.Range(0, _paperSprite.Length)];
            _spriteImage.color = DevTypeToColor(taskType);
        }
        
        public float GetExtrasShowDuration()
        {
            // сколько времени "минимально красиво" держать карточку на месте после OnBugResult
            // если текста/иконок нет — 0
            // иконки показываются ступенчато: duration * icons
            int currentExtrasShown = CountActiveExtras(); // посчитай активные _extraSprites
            if (currentExtrasShown <= 0 && !_extraSpriteText.gameObject.activeSelf)
                return 0f;

            // у тебя ShowExtraSprite даёт ступенчатую анимацию: duration * icons
            int icons = Mathf.Clamp(currentExtrasShown, 0, _extraSprites.Length);
            float duration = _showDuration;
            // Возьмём небольшой запас (например +0.1s) на завершение текста
            return icons * duration + 0.1f;
        }

        private int CountActiveExtras()
        {
            int c = 0;
            foreach (var img in _extraSprites)
                if (img != null && img.gameObject.activeSelf) c++;
            return c;
        }

        public void HideTask(Action onComplete = null)
        {
            if (_isOnStart || _isDestroyed)
            {
                onComplete?.Invoke();
                return;
            }

            _imageSequence?.Kill();
            _extraSpriteSequence?.Kill();
            _fxTextSequence?.Kill();

            //var extras = BuildHideExtrasSequence();

            _imageSequence = DOTween.Sequence();
            //_imageSequence.Append(extras); 
            _imageSequence.Append(_spriteImage.transform.DOScale(Vector3.zero, _hideDuration))
                .OnComplete(() => onComplete?.Invoke());
        }

        
        public async Task HideTaskAsync()
        {
            if (_isOnStart || _isDestroyed)
                return;

            _imageSequence?.Kill();
            _extraSpriteSequence?.Kill();
            _fxTextSequence?.Kill();

            var extras = BuildHideExtrasSequence();
            await extras.AsyncWaitForCompletion();

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

        public void ShowExtraSprite(int extraSpriteCount)
        {
            HideAllExtras(); 

            _extraSpriteSequence?.Kill();
            _extraSpriteSequence = DOTween.Sequence();

            _extraSpriteText.gameObject.SetActive(true);

            int icons = Mathf.Max(0, extraSpriteCount - 1);
            float duration = _showDuration;

            for (int i = 0; i < icons && i < _extraSprites.Length; i++)
            {
                var img = _extraSprites[i];
                img.transform.localScale = Vector3.zero;
                img.gameObject.SetActive(true);

                int capturedIndex = i; 
                _extraSpriteSequence.Insert(duration * i,
                    img.transform.DOScale(Vector3.one, duration).OnComplete(() =>
                    {
                        _extraSpriteText.transform.DOPunchRotation(new Vector3(0, 0, 15f), duration, 50);
                        _extraSpriteText.text = "x" + extraSpriteCount; 
                    })
                );
            }
        }
        
        private void HideAllExtras()
        {
            _extraSpriteSequence?.Kill();

            if (_extraSprites != null)
            {
                foreach (var img in _extraSprites)
                {
                    if (img == null) continue;
                    img.transform.localScale = Vector3.zero; 
                    img.gameObject.SetActive(false);
                }
            }

            if (_extraSpriteText != null)
            {
                _extraSpriteText.gameObject.SetActive(false);
                _extraSpriteText.text = string.Empty;
                _extraSpriteText.transform.localScale = Vector3.one;
                _extraSpriteText.transform.localRotation = Quaternion.identity;
            }
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
            if(_isDestroyed) return;
            
            if (_fxImage)
            {
                _fxImage.gameObject.SetActive(false);
                _fxImage.transform.localRotation = Quaternion.identity;
            }
           
            _fxTextSequence?.Kill();
            if(_fxImage) _fxText.gameObject.SetActive(false);
        }

        public void SetBugVisual(bool isBug)
        {
            if (_spriteImage == null) return;

            if (isBug)
            {
                _localEvents?.TriggerBugCreated();
                _spriteImage.color = Color.red;
            }
        }

        public void SetUnsuccessTask()
        {
            if (_spriteImage != null)
            {
                _spriteImage.color = Color.black;   
            }
        }
    }
}