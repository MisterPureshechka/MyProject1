using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Messenger
{
    public class MiniMessageButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _rect;
        public RectTransform RectTransform => _rect;
        public void ChangeMessageCount(int messageCount)
        {
            _text.text = messageCount.ToString();
        }
        
       
    }
}