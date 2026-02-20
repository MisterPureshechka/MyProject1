using System;
using System.Collections.Generic;
using Core;
using Scripts.Bugs;
using Scripts.GlobalStateMachine;
using Scripts.Progress;

namespace Scripts.Tasks
{
    public class TaskLibrary : ICleanUp
    {
        private readonly Dictionary<System.Enum, bool> _allTaskTypes = new();
        private readonly Dictionary<DevTaskType, List<IDevTask>> _allDevTasks = new();
        private readonly ProgressDataAdapterOLD _progressDataAdapterOld;
        private readonly BugLogic _bugLogic;
        private LocalEvents _localEvents;

        public TaskLibrary(ProgressDataAdapterOLD progressDataAdapterOld, BugLogic bugLogic, LocalEvents localEvents)
        {
            _progressDataAdapterOld = progressDataAdapterOld;
            _bugLogic = bugLogic;
            _localEvents = localEvents;
            LoadAllDevTasks();
        }

        

        private void LoadAllDevTasks()
        {
            _allDevTasks.Clear();

            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
            {
                _allDevTasks[type] = new List<IDevTask>();
            }

            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapterOld, DevTaskType.Programming, "Saving System", 100f));
            
            
        }

        public Dictionary<DevTaskType, List<IDevTask>> GetAlLDevTasks()
        {
            return _allDevTasks;
        }


        public void CleanUp()
        {
            _allTaskTypes.Clear();
        }

    }
}