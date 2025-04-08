namespace Scripts.Tasks
{
    public interface IDevTask : ITask
    {
        DevTaskType Type { get; set; }
    }
}