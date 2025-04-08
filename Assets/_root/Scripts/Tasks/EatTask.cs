using System;

namespace Scripts.Tasks
{
    public class EatTask : IEatTask
    {
        public event Action<ITask> OnTaskCompleted;
        public EatTaskType Type { get; set; }

        public string Id { get; }
        public string Title { get; }
        public float Progress { get; set; }
        public bool IsCompleted { get; }

        public EatTask(EatTaskType taskType, string title, float progress)
        {
            Type = taskType;
            Title = title;
            Progress = progress;
        }
        
        public void ApplyProgress(float delta)
        {
            Progress = Math.Max(0, Progress - delta);
            if (IsCompleted) OnTaskCompleted?.Invoke(this);
        }
    }
}