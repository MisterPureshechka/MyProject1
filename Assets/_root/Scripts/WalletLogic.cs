using Core;
using Scripts.Progress;

namespace Scripts
{
    public class WalletLogic : ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private int _walletAmount;
        
        private const string WalletKey = "WalletAmount";

        public WalletLogic(ProgressDataAdapter progressDataAdapter)
        {
            _progressDataAdapter = progressDataAdapter;
            
            _walletAmount = (int)_progressDataAdapter.GetMetadata(WalletKey).Value;
        }
        public bool TrySpend(int value)
        {
            if (_walletAmount >= value)
            {
                _walletAmount -= value;
                return true;
            }

            return false;
        }
        
        public void CleanUp()
        {
            
        }
    }
}