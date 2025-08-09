using System;
using System.Collections.Generic;

namespace Scripts.Wallet
{
    [Serializable]
    public class WalletConfig
    {
        public int Amount;
        public List<Transaction> Transactions;
    }
}