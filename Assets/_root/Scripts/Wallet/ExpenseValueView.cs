using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Wallet
{
    public class ExpenseValueView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _expenseName;
        [SerializeField] private TextMeshProUGUI _expenseValue;

        [SerializeField] private Color[] _colorKeys;

        public void SetInfo(string valueName, int value, bool isExpense)
        {
            _expenseName.text = valueName;
            
            _expenseValue.text = "$" + value;
            _expenseValue.color = isExpense ? _colorKeys[1] : _colorKeys[0];
        }
    }
}