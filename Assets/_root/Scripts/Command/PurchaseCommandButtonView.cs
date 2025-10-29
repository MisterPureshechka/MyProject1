using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using Scripts.Ui;

namespace Scripts.Tasks
{
    public class PurchaseCommandButtonView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _priceText;
        [field: SerializeField] public Button TaskPannelButton;
        [SerializeField] private BaseButtonAnimation _animationButton;

        private LocalEvents _events;
        private InteractiveObjectType _ioType;
        private string _defaultText;
        private Action _onExecute;
        private Color _defaultColor;
        private int _price;
        
        private bool _awaitingResult;      
        private Sequence _seq;    

        public InteractiveObjectType IoType => _ioType;

        public void Init(string label, int price, InteractiveObjectType ioType, LocalEvents eventsHub, Action onExecute)
        {
            _events   = eventsHub;
            _ioType   = ioType;
            _onExecute = onExecute;
            _price    = price;

            _defaultText = label;
            _text.text = label;
            _priceText.text = price.ToString();
            _defaultColor = _text.color;

            _awaitingResult = false;
            KillSeq(); // на всякий случай

            if (_events != null) _events.OnPurchaseUpgradeResult -= OnPurchaseResult;
            if (_events != null) _events.OnPurchaseUpgradeResult += OnPurchaseResult;

            TaskPannelButton.onClick.RemoveAllListeners();
            TaskPannelButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _onExecute?.Invoke(); 
        }

        private void OnPurchaseResult(InteractiveObjectType type, bool success)
        {
            if (type != _ioType) return; 

            if (!success)
                ShowNotEnoughMoney();
        }

        public void ShowNotEnoughMoney()
        {
            KillSeq();

            _priceText.text  = "=(";
            _priceText.color = Color.red;
            _text.text = "Not enough money";
            
            _seq = DOTween.Sequence().SetLink(gameObject);

            _seq.Append(_priceText.rectTransform.DOShakePosition(0.12f, 6f, 50, 0));
            _seq.Join(_text.rectTransform.DOShakePosition(0.12f, 6f, 50, 0));
            _seq.AppendInterval(2f); 

            _seq.AppendCallback(() =>
            {
                _seq.Append(_priceText.rectTransform.DOShakePosition(0.12f, 6f, 50, 0));
                _seq.Join(_text.rectTransform.DOShakePosition(0.12f, 6f, 50, 0));
                _priceText.text  = _price.ToString();
                _priceText.color = _defaultColor;
                _text.text = _defaultText;
            });
        }
        private void KillSeq()
        {
            if (_seq != null)
            {
                _seq.Kill(complete: false);
                _seq = null;
            }
            // гасим возможные подвисшие твины на этом rect
            _priceText?.rectTransform?.DOKill();
        }

        private void OnDestroy()
        {
            if (_events != null)
                _events.OnPurchaseUpgradeResult -= OnPurchaseResult;
        }
    }
}
