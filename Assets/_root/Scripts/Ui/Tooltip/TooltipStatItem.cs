using TMPro;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class TooltipStatItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _valueText;

        public void SetInfo(string title, float value)
        {
            _title.text = title;
            _valueText.text = value.ToString("F1") + "%"; 
        }
    }
}