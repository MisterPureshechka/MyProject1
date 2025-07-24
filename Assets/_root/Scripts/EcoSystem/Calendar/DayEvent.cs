using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Scripts.EcoSystem.Calendar
{
    public class DayEvent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void UpdateEventInfo(string text, Color color)
        {
            _text.text = text;
            _text.color = color;
        }
    }
}