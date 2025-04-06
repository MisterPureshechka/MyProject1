using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Tasks
{
    public class SprintSystem
    {

        public SprintSystem()
        {
            var programmingTask = new DevTask(DevTaskType.Programming, "Programming task", 100f);
            var devSprint = new DevSprint(4);
            var eatSprint = new EatSprint(3);

            devSprint.TryAddTask(programmingTask);
        }
    }


    public class DevSprint : Sprint<IDevTask> 
    {
        public override SprintType Type => SprintType.Dev;
        public DevSprint(int capacity) : base(capacity) { }
        
    }

    public class EatSprint : Sprint<IEatTask>
    {
        public override SprintType Type => SprintType.Eat;
        public EatSprint(int capacity) : base(capacity) { }
    }
    public abstract class SprintBase
    {
        public abstract SprintType Type { get; }
        public abstract IReadOnlyList<ITask> GetTasks();
        public abstract int Capacity { get; }
        public abstract int FreeSlots { get; }
        public abstract bool TryAddTask(ITask task);
    }

    public class Sprint<T> : SprintBase where T : ITask
    {
        
        public List<T> Tasks = new();
        public override SprintType Type { get; }
        public override IReadOnlyList<ITask> GetTasks() => Tasks.Cast<ITask>().ToList();
        
        public IReadOnlyList<T> GetTypedTasks() => Tasks.AsReadOnly();
        public override int Capacity { get; }
        public override int FreeSlots => Capacity - Tasks.Count;

        public Sprint(int capacity) => Capacity = capacity;

        public override bool TryAddTask(ITask task)
        {
            if (task is not T typedTask || FreeSlots <= 0) 
                return false;
        
            Tasks.Add(typedTask);
            return true;
        }

    }

    public interface IDevTask : ITask
    {
        DevTaskType Type { get; set; }
    }

    public interface IEatTask : ITask
    {
        EatTaskType Type { get; set; }
    }

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
    
    public interface ITask
    {
        string Id { get; }
        string Title { get; }
        float Progress { get; set; } 
        bool IsCompleted { get; }
        void ApplyProgress(float delta);
        
        event Action<ITask> OnTaskCompleted;
    }

    public enum SprintType
    {
        Dev,
        OfficeWork,
        Eat,
        Play,
        Talk
    }
    
    public enum DevTaskType
    {
        Programming,
        Art,
        GameDesign,
        SoundDesign,
    }

    public enum EatTaskType
    {
        coffee,
        cake,
    }
}