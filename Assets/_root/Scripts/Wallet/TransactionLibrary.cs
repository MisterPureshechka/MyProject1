using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Wallet
{
    public class TransactionLibrary
    {
        private readonly List<Transaction> _transactions = new();
        public IReadOnlyList<Transaction> All => _transactions;

        private TransactionLibrary() { }
        public static TransactionLibrary LoadFromResources(string resourcePath = "Meta/wallet_transaction")
        {
            var lib = new TransactionLibrary();

            var ta = Resources.Load<TextAsset>(resourcePath);
            if (ta == null)
            {
                Debug.LogError($"wallet_schedule.json not found at Resources/{resourcePath}.json");
                return lib;
            }

            var wrapper = JsonUtility.FromJson<WalletConfig>(ta.text);
            if (wrapper?.Transactions != null)
            {
                lib._transactions.AddRange(wrapper.Transactions);
            }
            else
            {
                Debug.LogWarning("wallet_transaction.json parsed, but Transactions is null/empty");
            }

            return lib;
        }
    }
}
