using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Job
{
    public class DevJob : IDevJob
    {
        public string CompanyName { get; set; }
        public string HRName { get; set; }
        public string JobTitle { get; }
        public int Salary { get; }
        public int[] SalaryDays { get; }
        public string Description { get; }
        public int WorkStartTime { get; }
        public Dictionary<DevTaskType, float> KnowledgeToGetJob { get; }

        public DevJob(string companyName, string hrName, string jobTitle, int salary, int[] salaryDays, string description, int workStartTime, Dictionary<DevTaskType, float> knowledgeToGetJob)
        {
            CompanyName = companyName;
            HRName = hrName;
            JobTitle = jobTitle;
            Salary = salary;
            Description = description;
            WorkStartTime = workStartTime;
            KnowledgeToGetJob = knowledgeToGetJob;
            SalaryDays = salaryDays;
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