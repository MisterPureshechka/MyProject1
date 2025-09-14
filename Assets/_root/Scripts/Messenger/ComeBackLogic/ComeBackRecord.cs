using System;
using _root.Notification;
using UnityEngine.Serialization;

namespace Scripts.Messenger.ComeBackLogic
{
    [Serializable]
    public sealed class ComeBackRecord
    {
        public string EventId;
        public CalendarEventType EventType;
        public bool Success;

        public int Year, Month, Day, Hour, Minute;

        public string CompanyName;
        public string HRName;
        public string JobTitle;
        public int Salary;
        public int[] SalaryDays;
        public int WorkStart;
    }
}