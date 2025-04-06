using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class TaskPanelButtonView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [field: SerializeField] public Button TaskPannelButton;

        public void UpdateInfo(string info)
        {
            _text.text = info;
        }
    }
}