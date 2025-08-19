using System;

namespace Scripts.Messenger
{
    public interface IMessageSender
    {
        string Id { get; set; }
        string Name { get; set; }
        string Message { get; set; } 
        Action OnAccept { get; set; }
    }
}