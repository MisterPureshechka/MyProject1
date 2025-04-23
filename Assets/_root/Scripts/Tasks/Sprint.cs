using System.Collections.Generic;
using System.Linq;

namespace Scripts.Tasks
{
    public class Sprint<T> : SprintBase where T : ITask
    {
        
        public List<T> Tasks = new();
        public override SprintType Type { get; }
        public override IReadOnlyList<ITask> GetTasks() => Tasks.Cast<ITask>().ToList();
        public override int Capacity { get; }
        public override int FreeSlots => Capacity - Tasks.Count;
        public override bool IsActiveSprint => Tasks.Any();
        public Sprint(int capacity) => Capacity = capacity;

        public override bool TryAddTask(ITask task)
        {
            if (task is not T typedTask || FreeSlots <= 0) 
                return false;
        
            Tasks.Add(typedTask);
            return true;
        }

        public override void ClearSprint()
        {
            Tasks.Clear();
        }
    }
}