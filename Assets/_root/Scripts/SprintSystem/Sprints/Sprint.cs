using System.Collections.Generic;
using System.Linq;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Tasks
{
    public class Sprint<T> : SprintBase where T : ITask
    {
        
        public List<T> Tasks = new();
        public override SprintType Type { get; }
        public override List<ITask> GetTasks() => Tasks.Cast<ITask>().ToList();
        public override int Capacity { get; }
        public override int FreeSlots => Capacity - Tasks.Count;
        public override bool IsActiveSprint => Tasks.Any();
        public override bool HasCatalog { get; }

        public override bool IsAutoSprint { get; }
        
        public Sprint(int capacity, IInteractiveObject objectToInteract) => Capacity = capacity;

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

        public override bool ShouldPersistTasksOnExit { get; }

        public override IInteractiveObject InteractiveObject { get; }
    }
}