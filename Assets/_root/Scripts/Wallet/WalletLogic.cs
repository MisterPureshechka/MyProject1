using Core;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Wallet
{
    public class WalletLogic : ICleanUp
    {
        private WalletButtonView _walletButtonView;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly GameProgress _gameProgress;
        private int _walletAmount;
        
        private const string WalletKey = "WalletAmount";

        public WalletLogic(ProgressDataAdapter progressDataAdapter, GameProgress gameProgress)
        {
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;
            _walletButtonView = Object.FindAnyObjectByType<WalletButtonView>();
            _walletAmount = (int)_progressDataAdapter.GetMetadata(WalletKey).Value;

            UpdateWallet();
        }

        private void UpdateWallet()
        {
            _walletButtonView.UpdateWallet(_walletAmount);
        }
        public bool TrySpend(int value)
        {
            if (_walletAmount >= value)
            {
                _walletAmount -= value;
                UpdateWallet();
                _progressDataAdapter.GetMetadata(WalletKey).Value -= value;
                _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());
                return true;
            }

            return false;
        }
        
        public void CleanUp()
        {
            
        }
    }
}