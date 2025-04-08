using System;

namespace Scripts.Tasks
{
    public interface ITask
    {
        string Id { get; }
        string Title { get; }
        float Progress { get; set; } 
        bool IsCompleted { get; }
        void ApplyProgress(float delta);
        
        event Action<ITask> OnTaskCompleted;
    }
}