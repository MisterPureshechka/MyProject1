using System.Collections.Generic;
using Scripts.Stat;
using Scripts.Tasks;

namespace Scripts.Job
{
    public interface IJob
    {
        public string JobId { get; }
        string CompanyName { get; }
        string HRName { get; }
        string JobTitle { get; }
        int Salary { get; }
        int[] SalaryDays { get; }
        string Description { get; }
        int WorkStartTime { get; }
        int HoursBeforeComeBack { get; }
        Dictionary<HealthStatType, float> HealthToUpdateAfter { get; }
        Dictionary<DevTaskType, float> KnowledgeToGetJob { get; } 
        Dictionary<DevTaskType, float> KnowledgeToUpdateAfter { get; }
        
    }
}