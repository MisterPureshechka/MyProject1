using System;

namespace Scripts.Messenger
{
    public class ScheduleMessageSender : IScheduleMessageSender
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public Action OnAccept { get; set; }
    }
}