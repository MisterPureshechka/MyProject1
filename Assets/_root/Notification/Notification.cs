using System;

namespace _root.Notification
{
    public class Notification : INotification
    {
        public string Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public Action AddCommand { get; set; }

        public Notification(string id, NotificationType notificationType, string title, string message, int year, int month, int day, int hour, int minute, Action addCommand = null)
        {
            Id = id;
            Type = notificationType;
            Title = title;
            Message = message;
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            AddCommand = addCommand;
        }
    }
}