using System.Collections.Generic;

namespace _root.Notification
{
    public class NotificationLibrary
    {
        private List<Notification> _notifications = new();

        public NotificationLibrary()
        {
            _notifications.Add(new Notification("1", NotificationType.Calendar,"Work", "Time to go for a work", 2025, 9, 7, 10, 5));
        }

        public List<Notification> GetNotifications()
        {
            return _notifications;
        }
    }
}