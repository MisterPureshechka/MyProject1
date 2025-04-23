using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class FaderView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        
        private Sequence _sequence;
        

        public void Show()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_image.DOFade(0.9f, 0.5f));
        }

        public void Hide()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_image.DOFade(0, 0.5f));
        }
    }
}