using System;

namespace _root.Notification
{
    public interface INotification 
    {
        string Id { get; set; }
        NotificationType Type { get; set; }
        string Title { get; set; }
        string Message { get; set; }
        
        int Year { get; set; }
        int Month { get; set; }
        int Day { get; set; }
        int Hour { get; set; }
        int Minute { get; set; }

    }
}