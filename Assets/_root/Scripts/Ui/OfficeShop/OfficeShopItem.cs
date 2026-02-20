using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.OfficeShop
{
    public class OfficeShopItem : MonoBehaviour
    {
        [field: SerializeField] public Button Button;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _price;

        public void Init(OfficeOffer offer)
        {
            _description.text = offer.Description;
            _price.text = offer.Price.ToString();
        }
    }
}