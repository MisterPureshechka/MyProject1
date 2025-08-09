using System;

namespace Scripts.Wallet
{
    [Serializable]
    public class Transaction
    {
        public int Id;
        public string Name;
        public string Description;
        public int Amount;
        public int DayForTransaction;
    }
}