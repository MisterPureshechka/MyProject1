using DG.Tweening;
using Scripts.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class TaskView : MonoBehaviour
    {
        private Sequence _sequence;
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

        private bool _isOnStart;
        
        public void SetInfo(string titleText, float progressText)
        {
            //_titleText.text = titleText;
            _progressInfo.text = progressText.ToString("0.0");
        }

        public void SetInfoIfDev(string titleText, float progressText, DevTaskType taskType)
        {
            //_titleText.text = titleText;
            _isOnStart = true;
            _progressInfo.text = progressText.ToString("0.0");
            _spriteImage.sprite = _paperSprite[Random.Range(0, _paperSprite.Length)];
            _spriteImage.color = DevTypeToColor(taskType);
        }
        

        private void OnDestroy()
        {
            _isDestroyed = true;
            _sequence?.Kill();
        }

        public void UpdateProgress(float progress, float value)
        {
            if (_isDestroyed) return;
        
            AnimateTextFx(value);
            _isOnStart = false;
            _progressInfo.text = progress.ToString("0.0");
        }

        private void AnimateTextFx(float value)
        {
            if(_isOnStart || _isDestroyed) return;
        
            _sequence?.Kill();
        
            if (_fxText == null || _fxText.transform == null) return;
        
            _sequence = DOTween.Sequence();
        
            var offset = Random.Range(-_offset, _offset);
            _fxText.text = value.ToString("0.0");
        
            if (this == null || transform == null) return;
        
            _fxText.transform.position = transform.position + new Vector3(offset, offset, 0);
        
            _sequence.Append(_fxText.DOFade(1, 0));
            _sequence.Append(_fxText.transform.DOMove(
                _fxText.transform.position + _moveToValue, 
                0.5f).SetEase(Ease.OutSine));
            _sequence.Join(_fxText.DOFade(0, 0.5f).SetEase(Ease.OutSine));
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