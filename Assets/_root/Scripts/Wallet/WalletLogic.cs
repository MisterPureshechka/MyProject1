using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
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
        
        private List<Transaction> _transactions = new();
        

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
        }

        private void UpdateMiniWallet()
        {
            _walletButtonView.UpdateWallet(_walletAmount);
        }

        private void UpdateWallet()
        {
            _transactions.Add(_transactionLibrary.All[0]);
            _transactions.Add(_transactionLibrary.All[1]);
            _transactions.Add(_transactionLibrary.All[2]);
            
            _walletCatalogue.UpdateInfo(_transactions, _walletAmount);
        }
        
        public bool TrySpend(int value)
        {
            if (_walletAmount >= value)
            {
                _walletAmount -= value;
                UpdateMiniWallet();
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