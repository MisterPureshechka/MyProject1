using System;

namespace _root.Notification
{
    [Serializable]
    public class CalendarEvent
    {
        public string Id;
        public string Name;
        public int Day;
        public int Month;
        public int Year;
        public int Hour;
        public int Minute;
        public string Message;
        public string ComeBackMessage;
        public Action OnExit;
    }
}