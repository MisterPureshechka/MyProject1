namespace Scripts.Messenger
{
    public interface IScheduleMessageSender : IMessageSender
    {
        int Year {get;set;}
        int Month {get;set;}
        int Day {get;set;}
        int Hour {get;set;}
        int Minute {get;set;}
        
    }
}