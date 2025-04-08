using System.Collections.Generic;

namespace Scripts.Tasks
{
    public abstract class SprintBase
    {
        public abstract SprintType Type { get; }
        public abstract IReadOnlyList<ITask> GetTasks();
        public abstract int Capacity { get; }
        public abstract int FreeSlots { get; }
        public abstract bool TryAddTask(ITask task);
    }
}