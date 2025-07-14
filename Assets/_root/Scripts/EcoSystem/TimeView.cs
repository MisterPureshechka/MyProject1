using TMPro;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class TimeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timeText;

        public void UpdateTimeText(float time)
        {
            int hours = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt((time - hours) * 60f);
            _timeText.text = $"{hours:00}:{minutes:00}";
        }
    }
}