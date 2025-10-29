using System;
using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

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
            _localEvents.OnWalletAmountIncrease += IncreaseWalletAmount;
            _localEvents.OnPayDay += DecreaseWalletAmount;
            _localEvents.OnNewTransaction += ApplyTransaction;
            _localEvents.OnPurchaseUpgradeRequested += OnPurchaseUpgradeRequested;
        }
        
        private void OnPurchaseUpgradeRequested(InteractiveObjectType type, int price)
        {
            bool success = TrySpend(price);
            _localEvents.TriggerPurchaseUpgradeResult(type, success);
        }

        private void DecreaseWalletAmount(int value)
        {
            _walletAmount -= value;
            
            _progressDataAdapter.TryUpdateValue(WalletKey, -value);
            
            UpdateWallet();
        }

        private void IncreaseWalletAmount(int amount)
        {
            _walletAmount += amount;
            
            _progressDataAdapter.TryUpdateValue(WalletKey, amount);
            
            Debug.Log($"[Salary] +{amount}. Баланс = " +
                      _progressDataAdapter.GetProgressData().Metadata.GetValue(WalletKey));

            UpdateWallet();
        }
        
        private void AddOrSwitchIncome(IDevJob job)
        {
            if (_currentJobIncome != null)
                RemoveIncomeTransaction(_currentJobIncome);

            var transaction = new Transaction
            {
                Amount = job.Salary,
                Description = job.Description,
                Name = job.JobTitle,
                DaysForTransaction = job.SalaryDays
            };

            _currentJobIncome = transaction;
            ApplyTransaction(transaction);

            UpdateWallet();
        }
        
        private void RemoveIncomeTransaction(Transaction transaction)
        {
            if (transaction == null) return;

            _icomeAndExpenses.Remove(transaction);
        }


        private void UpdateMiniWallet()
        {
            _walletButtonView.UpdateWallet(_walletAmount);
        }

        private void UpdateWallet()
        {
            _walletCatalogue.UpdateInfo(_icomeAndExpenses, _walletAmount);
            UpdateMiniWallet();
        }

        private void ApplyTransaction(Transaction transaction)
        {
            foreach (var dayForTransaction in transaction.DaysForTransaction)
            {
                _localEvents.TriggerCalendarNoteAdded(transaction.Description, dayForTransaction);
            }
            
            _icomeAndExpenses.Add(transaction);
            UpdateWallet();
        }
        
        public bool TrySpend(int value)
        {
            if (value <= 0) return true;
            if (_walletAmount < value) return false;

            _walletAmount -= value;
            _progressDataAdapter.TryUpdateValue(WalletKey, -value);
            _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());

            UpdateMiniWallet();
            UpdateWallet();
            return true;
        }

        
        public void CleanUp()
        {
            _localEvents.OnWalletUpdate -= UpdateWallet;
            _walletButtonView.Button.onClick.RemoveAllListeners();
            _localEvents.OnNewJobFound -= AddOrSwitchIncome;
            _localEvents.OnWalletAmountIncrease -= IncreaseWalletAmount;
            _localEvents.OnPayDay -= DecreaseWalletAmount;
            _localEvents.OnNewTransaction -= ApplyTransaction;
            _localEvents.OnPurchaseUpgradeRequested -= OnPurchaseUpgradeRequested;
        }
    }
}