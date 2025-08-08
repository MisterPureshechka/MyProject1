namespace _root.Notification
{
    public class Notification : INotification
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }

        public Notification(string id, string title, string message, int year, int month, int day, int hour, int minute)
        {
            Id = id;
            Title = title;
            Message = message;
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }
    }
}