using System;

namespace Scripts.Wallet
{
    [Serializable]
    public class Transaction
    {
        public int Id;
        public string Name;
        public int Amount;
        public int DayForTransaction;
    }
}