using System;
using System.Collections.Generic;
using Scripts.Stat;
using Scripts.Tasks;

namespace Scripts.Job
{
    public class DevJob : IDevJob
    {
        public string xJobId => $"{CompanyName}|{JobTitle}"
            .ToLowerInvariant()
            .Replace(" ", "_");

        public string JobId { get; }
        public string CompanyName { get; set; }
        public string HRName { get; set; }
        public string JobTitle { get; }
        public int Salary { get; }
        public int[] SalaryDays { get; }
        public string Description { get; }
        public int WorkStartTime { get; }
        public int HoursBeforeComeBack { get; }
        public Dictionary<HealthStatType, float> HealthToUpdateAfter { get; set; }
        public Dictionary<DevTaskType, float> KnowledgeToGetJob { get; set; }
        public Dictionary<DevTaskType, float> KnowledgeToUpdateAfter { get; set; }

        public DevJob(string companyName, string hrName, string jobTitle, int salary, int[] salaryDays, string description, int workStartTime, Dictionary<HealthStatType, float> healthToUpdateAfter, Dictionary<DevTaskType, float> knowledgeToGetJob,  Dictionary<DevTaskType, float> knowledgeToUpdateAfter, int hoursBeforeComeBack)
        {
            JobId = new Guid().ToString();
            CompanyName = companyName;
            HRName = hrName;
            JobTitle = jobTitle;
            Salary = salary;
            Description = description;
            WorkStartTime = workStartTime;
            KnowledgeToGetJob = knowledgeToGetJob;
            SalaryDays = salaryDays;
            HealthToUpdateAfter = healthToUpdateAfter;
            KnowledgeToUpdateAfter = knowledgeToUpdateAfter;
            HoursBeforeComeBack = hoursBeforeComeBack;
        }

        public bool TryGetJob(Dictionary<DevTaskType, float> currentKnowledge)
        {
            foreach (var requirement in KnowledgeToGetJob)
            {

                if (!currentKnowledge.TryGetValue(requirement.Key, out var playerValue) 
                    || playerValue < requirement.Value)
                {
                    return false;
                }
            }

            return true; 
        }
    }
}