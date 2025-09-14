namespace Scripts.Job
{
    public interface IJob
    {
        string CompanyName { get; }
        string HRName { get; }
        string JobTitle { get; }
        int Salary { get; }
        int[] SalaryDays { get; }
        string Description { get; }
        int WorkStartTime { get; }
    }
}