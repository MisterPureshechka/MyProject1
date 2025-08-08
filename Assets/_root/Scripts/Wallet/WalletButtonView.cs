using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Wallet
{
    public class WalletButtonView : MonoBehaviour
    {
        [field: SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        public void UpdateWallet(int amount)
        {
            _text.text = amount + "$";
        }
    }
}