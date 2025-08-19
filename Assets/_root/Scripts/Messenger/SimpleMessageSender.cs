using System;

namespace Scripts.Messenger
{
    public class SimpleMessageSender : IMessageSender
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public Action OnAccept { get; set; }

        public SimpleMessageSender(string name, string message, Action onAccept = null)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Message = message;
            OnAccept = onAccept;
        }
    }
}