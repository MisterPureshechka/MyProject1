using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Planning
{
    public class ConnectorView : MonoBehaviour
    {
        public enum ConnectionState
        {
            Inactive,
            Available,
            Completed
        }

        [SerializeField] private Image _image;
        [SerializeField] private Color _inactiveColor = Color.gray;
        [SerializeField] private Color _availableColor = Color.white;
        [SerializeField] private Color _completedColor = Color.green;
        [SerializeField] private Color _selectColor = Color.yellow;

        public Image Image => _image;

        public void SetState(ConnectionState state)
        {
            Color target;

            switch (state)
            {
                case ConnectionState.Available:
                    target = _availableColor;
                    break;
                case ConnectionState.Completed:
                    target = _completedColor;
                    break;
                default:
                    target = _inactiveColor;
                    break;
            }

            _image.color = target;
        }

        public void PlaySelectAnimation(System.Action onComplete)
        {
            _image.DOKill();

            // Примитивная анимация: моргнуть цветом и вернуться к исходному
            Color startColor = _image.color;

            _image
                .DOColor(_selectColor, 0.25f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    _image.color = startColor;
                    onComplete?.Invoke();
                });
        }
    }
}