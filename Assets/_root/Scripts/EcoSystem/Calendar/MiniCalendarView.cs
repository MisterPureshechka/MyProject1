using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.EcoSystem.Calendar
{
    public class MiniCalendarView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentMonth;
        [SerializeField] private TextMeshProUGUI _currentDay;
        [SerializeField] private Image _miniCalendarIcon;


        public void UpdateMiniCalendar(int month, int day)
        {
            UpdateDay(day);
            UpdateMonth(month);
        }
        private void UpdateMonth(int month)
        {
            _currentMonth.text = CalendarConsts.GetMiniMonthName(month);
        }

        private void UpdateDay(int day)
        {
            var dayOfWeek = CalendarConsts.GetMiniDayName(day);
            _currentDay.text = $"{day} {dayOfWeek}";
        }
    }
}