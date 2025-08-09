using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Wallet
{
    public class WalletButtonView : MonoBehaviour
    {
        [FormerlySerializedAs("_button")] [field: SerializeField] public Button Button;
        [SerializeField] private TextMeshProUGUI _text;

        public void UpdateWallet(int amount)
        {
            _text.text = amount + "$";
        }
    }
}