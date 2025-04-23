using System;

namespace Scripts.Tasks
{
    public interface ITask : IPanelItem
    {
        string Id { get; }
        float Progress { get; set; } 
        bool IsCompleted { get; }
        void ApplyProgress(float delta, float interval = 0f);

        event Action<ITask> OnTaskCompleted;
        event Action<ITask, float> OnProgressChanged;

        ITask Clone();
    }
}