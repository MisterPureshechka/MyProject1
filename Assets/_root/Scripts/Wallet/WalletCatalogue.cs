using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Wallet
{
    public class WalletCatalogue : MonoBehaviour, ICatalogue
    {
        [SerializeField] private RectTransform _walletRect;
        [SerializeField] private RectTransform _expenseContainer;
        [SerializeField] private LayoutGroup _layoutGroup;
        [SerializeField] private ContentSizeFitter _sizeFitter;
        [SerializeField] private ExpenseValueView _expensePrefab;
        [SerializeField] private ExpenseValueView _totalExpenses;
        [SerializeField] private GameObject _line;
        [SerializeField] private TextMeshProUGUI _currentBalance;
        [SerializeField] private Button _closeButton;
        
        
        [SerializeField] private Vector3 _showPosition = new Vector3(0, -75f, 0);
        [SerializeField] private Vector3 _hidePosition = new Vector3(0, -400f, 0);
        
        private LocalEvents _localEvents;

        private void Start()
        {
            _closeButton.onClick.AddListener(() => _localEvents.TriggerHideCatalogue(this));
            _closeButton.gameObject.SetActive(false);
            gameObject.SetActive(true);
            _walletRect.localPosition = _hidePosition;
            _sizeFitter.enabled = false;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_walletRect); 
        }

        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            
        }

        public void UpdateInfo(List<Transaction> transactions, int balance)
        {
            for (int i = _expenseContainer.childCount - 2; i >= 0; i--)
                Destroy(_expenseContainer.GetChild(i).gameObject);

            var sorted = transactions
                .OrderByDescending(t => t.Amount >= 0) 
                .ThenByDescending(t => t.Amount)      
                .ToList();
            
            var totalExpenses = 0;
            
            for (int i = 0; i < sorted.Count; i++)
            {
                var transaction = sorted[i];

                var view = Instantiate(_expensePrefab, _expenseContainer);
                view.SetInfo(transaction.Name, transaction.Amount, transaction.Amount < 0);

                totalExpenses += transaction.Amount;

                if (i < sorted.Count - 1)
                    Instantiate(_line, _expenseContainer);
            }
            
            _totalExpenses.transform.SetAsLastSibling();
            _totalExpenses.SetInfo("Total: ", totalExpenses, totalExpenses < 0);
            _currentBalance.text = "$" + balance;
        }
        
        public async void Show(Action onComplete = null)
        {
            gameObject.SetActive(true);
            
            await Task.Yield();
            
            _layoutGroup.enabled = true;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_walletRect); 
            LayoutRebuilder.ForceRebuildLayoutImmediate(_expenseContainer);
            _sizeFitter.enabled = true;
            _closeButton.gameObject.SetActive(true);
            _walletRect.DOLocalMoveY(_showPosition.y, 0.4f).SetEase(Ease.OutSine)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Hide(Action onComplete = null)
        {
            _walletRect.DOLocalMoveY(_hidePosition.y, 0.4f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }

        public bool IsVisible { get; }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
        }
    }
}