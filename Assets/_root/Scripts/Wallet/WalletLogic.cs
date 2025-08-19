using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Wallet
{
    public class WalletLogic : ICleanUp
    {
        private WalletButtonView _walletButtonView;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly GameProgress _gameProgress;
        private readonly LocalEvents _localEvents;
        private readonly WalletCatalogue _walletCatalogue;
        private int _walletAmount;
        
        private const string WalletKey = "WalletAmount";
        private Dictionary<string, int> _walletAmountDictionary;
        
        private TransactionLibrary _transactionLibrary;
        
        private List<Transaction> _icomeAndExpenses = new();
        private Transaction _currentJobIncome;

        public WalletLogic(ProgressDataAdapter progressDataAdapter, GameProgress gameProgress, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;
            _localEvents = localEvents;
            _walletButtonView = Object.FindAnyObjectByType<WalletButtonView>();
            _walletAmount = (int)_progressDataAdapter.GetMetadata(WalletKey).Value;
            _walletCatalogue = Object.FindAnyObjectByType<WalletCatalogue>(FindObjectsInactive.Include);
            _walletCatalogue.Init(_localEvents);
            
            _transactionLibrary = TransactionLibrary.LoadFromResources();
            
            UpdateMiniWallet();
            UpdateWallet();

            _walletButtonView.Button.onClick.AddListener(() => _localEvents.TriggerShowCatalogue(_walletCatalogue));
            _localEvents.OnWalletUpdate += UpdateWallet;
            _localEvents.OnNewJobFound += AddOrSwitchIncome;
        }

        private void AddOrSwitchIncome(IDevJob obj)
        {
            // if (_currentJobIncome != null)
            // {
            //     _currentJobIncome.
            // }
        }

        private void UpdateMiniWallet()
        {
            _walletButtonView.UpdateWallet(_walletAmount);
        }

        private void UpdateWallet()
        {
            // ApplyTransaction(_transactionLibrary.All[0]);
            // ApplyTransaction(_transactionLibrary.All[1]);
            // ApplyTransaction(_transactionLibrary.All[2]);
            
            _walletCatalogue.UpdateInfo(_icomeAndExpenses, _walletAmount);
        }

        private void ApplyTransaction(Transaction transaction)
        {
            _localEvents.TriggerCalendarNoteAdded(transaction.Description, transaction.DayForTransaction);
            _icomeAndExpenses.Add(transaction);
        }
        
        public bool TrySpend(int value)
        {
            if (_walletAmount >= value)
            {
                _walletAmount -= value;
                UpdateMiniWallet();
                UpdateWallet();
                _progressDataAdapter.GetMetadata(WalletKey).Value -= value;
                _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());
                return true;
            }

            return false;
        }
        
        public void CleanUp()
        {
            _localEvents.OnWalletUpdate -= UpdateWallet;
            _walletButtonView.Button.onClick.RemoveAllListeners();
        }
    }
}