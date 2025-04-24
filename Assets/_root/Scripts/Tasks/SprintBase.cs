using System.Collections.Generic;

namespace Scripts.Tasks
{
    public abstract class SprintBase : ISprint
    {
        public abstract SprintType Type { get; }
        public abstract int Capacity { get; }
        public abstract bool IsActiveSprint { get; }
        public abstract int FreeSlots { get; }
        public abstract bool TryAddTask(ITask task);
        public abstract List<ITask> GetTasks();

        public abstract void ClearSprint();
        public abstract bool ShouldPersistTasksOnExit { get; }
    }
    
    public interface ISprint
    {
        SprintType Type { get; }
        int Capacity { get; }
        bool TryAddTask(ITask task);
        List<ITask> GetTasks();
        void ClearSprint();
        bool ShouldPersistTasksOnExit { get; } // новое!
    }
}