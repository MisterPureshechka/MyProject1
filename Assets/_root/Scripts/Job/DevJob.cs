using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Job
{
    public class DevJob : IDevJob
    {
        public string Name { get; }
        public int Salary { get; }
        public int SalaryDay { get; }
        public string Description { get; }
        public int WorkStartTime { get; }
        public Dictionary<DevTaskType, float> KnowledgeToGetJob { get; }

        public DevJob(string name, int salary, int salaryDay, string description, int workStartTime, Dictionary<DevTaskType, float> knowledgeToGetJob)
        {
            Name = name;
            Salary = salary;
            Description = description;
            WorkStartTime = workStartTime;
            KnowledgeToGetJob = knowledgeToGetJob;
            SalaryDay = salaryDay;
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