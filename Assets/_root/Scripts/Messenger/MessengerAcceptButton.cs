using DG.Tweening;
using Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Messenger
{
    public class MessengerAcceptButton : MonoBehaviour
    {
        public Button Button;
        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        [SerializeField] private BaseButtonAnimation _buttonAnimator;

        private Sequence _sequence;

        private bool _isActive;

        [ContextMenu("Switch")]
        public void SwitchToggle()
        {
            if (_image.transform.localScale == Vector3.zero == false)
                _image.transform.localScale = Vector3.zero;

            Button.interactable = false;
            
            if(!_isActive) return;
            
            _image.gameObject.SetActive(true);
            
            
            _sequence?.Kill(true);
            _sequence = DOTween.Sequence().SetLink(gameObject);

            _sequence.Append(_text.transform.DOScale(0f, 0.15f).SetEase(Ease.InSine));
            _sequence.Join(_text.DOFade(0.0f, 0.12f));
            
            _sequence.Join(_image.transform
                .DOScale(1f, 0.35f)
                .SetEase(Ease.OutBack, 1.6f));
            _sequence.OnComplete(() => _buttonAnimator.SetButtonActive(false));
        }

        public void SwitchToggleImmediate(bool wasButtonPressed)
        {
            if (wasButtonPressed)
            {
                _image.gameObject.SetActive(true);
                _image.DOFade(1f, 0f);
                _image.transform.localScale = Vector3.one;
                _text.gameObject.SetActive(false);
                _buttonAnimator.SetButtonActive(false);
                _isActive = false;
                Button.interactable = false;
            }
            else
            {
                ResetButton();
            }
        }

        public void DisableButton()
        {
            _text.gameObject.SetActive(true);
            _text.transform.localScale = Vector3.one;
            _text.DOFade(0.2f, 0f);
            _image.gameObject.SetActive(false);
            _buttonAnimator.SetButtonActive(false);
            Button.interactable = false;
            _isActive = false;
        }

        public void ResetButton()
        {
            Button.interactable = true;
            _isActive = true;
            _buttonAnimator.SetButtonActive(true);
            _text.gameObject.SetActive(true);
            _text.DOFade(1f, 0);
            _text.transform.localScale = Vector3.one;
            _image.transform.localScale = Vector3.zero;
        }
        
    }
}