namespace Scripts.Tasks
{
    public interface IEatTask : ITask
    {
        EatTaskType Type { get; set; }
    }
}