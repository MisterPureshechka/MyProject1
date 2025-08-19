using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Job
{
    public interface IDevJob : IJob
    {
        Dictionary<DevTaskType, float> KnowledgeToGetJob { get; } 
        
        bool TryGetJob(Dictionary<DevTaskType, float> knowledgeToGetJob);
    }
}