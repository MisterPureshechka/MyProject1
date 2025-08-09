using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Wallet
{
    /// <summary>
    /// Загружает расписание транзакций из Resources и даёт удобные выборки.
    /// </summary>
    public class TransactionLibrary
    {
        private readonly List<Transaction> _transactions = new();
        public IReadOnlyList<Transaction> All => _transactions;

        private TransactionLibrary() { }

        /// <summary>
        /// Грузит wallet_schedule.json из Resources (по умолчанию "Meta/wallet_transaction").
        /// </summary>
        public static TransactionLibrary LoadFromResources(string resourcePath = "Meta/wallet_transaction")
        {
            var lib = new TransactionLibrary();

            var ta = Resources.Load<TextAsset>(resourcePath);
            if (ta == null)
            {
                Debug.LogError($"wallet_schedule.json not found at Resources/{resourcePath}.json");
                return lib;
            }

            // Обёртка под JsonUtility
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

        /// <summary>
        /// Возвращает транзакции, которые должны сработать в указанный день месяца.
        /// </summary>
        public IEnumerable<Transaction> GetForDay(int dayOfMonth)
        {
            // На всякий случай ограничим диапазон
            if (dayOfMonth < 1 || dayOfMonth > 31) return Enumerable.Empty<Transaction>();
            return _transactions.Where(t => t.DayForTransaction == dayOfMonth);
        }

        /// <summary>
        /// Сводка по категориям за «месяц» (ты отдаёшь список фактически сработавших транзакций).
        /// Возвращает словарь: Название -> сумма (можно отфильтровать только расходы/доходы).
        /// </summary>
        public static Dictionary<string, int> BuildSummaryByName(
            IEnumerable<Transaction> monthTransactions, bool expensesOnly = false, bool incomesOnly = false)
        {
            var filtered = monthTransactions.Where(t =>
                (!expensesOnly || t.Amount < 0) &&
                (!incomesOnly || t.Amount > 0));

            return filtered
                .GroupBy(t => t.Name)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        }
    }
}
