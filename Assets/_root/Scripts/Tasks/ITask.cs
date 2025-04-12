using System;

namespace Scripts.Tasks
{
    public interface ITask : IPanelItem
    {
        string Id { get; }
        float Progress { get; set; } 
        bool IsCompleted { get; }
        void ApplyProgress(float delta);
        
        event Action<ITask> OnTaskCompleted;
    }
}