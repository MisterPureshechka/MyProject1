using TMPro;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class TaskButtonsContainerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void UpdateText(string text)
        {
            _text.text = text;
        }
    }
}