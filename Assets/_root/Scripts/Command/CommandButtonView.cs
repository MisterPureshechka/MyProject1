using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class CommandButtonView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [field: SerializeField] public Button TaskPannelButton;

        public void UpdateInfo(string info)
        {
            _text.text = info;
        }
    }
}