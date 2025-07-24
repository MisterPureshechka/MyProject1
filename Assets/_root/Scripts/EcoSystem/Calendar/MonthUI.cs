using System.Collections.Generic;
using UnityEngine;

namespace Scripts.EcoSystem.Calendar
{
    public class MonthUI 
    {
        private List<DayUI> _dayHolders = new();

        public void AddDay(DayUI dayUI)
        {
            _dayHolders.Add(dayUI);
        }
    }
}