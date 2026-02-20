using _root.Scripts.Rooms.RoomItems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.ItemShop
{
    public class ItemShopItem : MonoBehaviour
    {
        public Button BuyButton;
        
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Transform _previewContainer;

        public void Init(RoomItemConfig roomItem)
        {
            _name.text = roomItem.Name;
            _price.text = roomItem.Cost.ToString();
            //_description.text = roomItem.Description;
            Instantiate(roomItem.Preview, _previewContainer); 
        }

        public void Destroy()
        {
            BuyButton.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}