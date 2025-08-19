namespace Scripts.Job
{
    public interface IJob
    {
        string Name { get; }
        int Salary { get; }
        int SalaryDay { get; }
        string Description { get; }
        int WorkStartTime { get; }
    }
}