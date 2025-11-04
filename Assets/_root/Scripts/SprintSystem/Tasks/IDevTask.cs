using System;

namespace Scripts.Tasks
{
    public interface IDevTask : ITask
    {
        DevTaskType Type { get; set; }

        event Action<ITask, int, bool> OnBugResult;
        int Result { get; }

        event Action<bool> BugStateChanged;
        bool IsBug { get; }
    }
}