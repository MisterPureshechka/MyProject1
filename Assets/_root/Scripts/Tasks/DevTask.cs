using System;

namespace Scripts.Tasks
{
    public class DevTask : IDevTask
    {
        public event Action<ITask> OnTaskCompleted;
        public DevTaskType Type { get; set; }
        
        public string Id { get; set;}
        public string Title { get; set; }
        public float Progress { get; set; }
        public bool IsCompleted { get; }

        public DevTask(DevTaskType taskType, string title, float progress)
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