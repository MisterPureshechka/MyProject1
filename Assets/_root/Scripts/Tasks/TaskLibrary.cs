using System.Collections.Generic;
using Core;
using Scripts.Progress;

namespace Scripts.Tasks
{
    public class TaskLibrary : ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        
        private Dictionary<DevTaskType, bool> _allDevTaskTypes = new();
        private Dictionary<EatTaskType, bool> _allEatTaskTypes = new();
        
        private Dictionary<IDevTask, bool> _allDevTasks = new();
        private Dictionary<IEatTask, bool> _allEatTasks = new();
        
        public TaskLibrary()
        {
            //_progressDataAdapter = progressDataAdapter;

            LoadAllAvailableTasks();
        }

        private void LoadAllAvailableTasks()
        {
            _allDevTasks.Add(new DevTask(DevTaskType.Programming, "Saving System", 100f), true);
            _allDevTasks.Add(new DevTask(DevTaskType.Programming, "Base Mechanics", 100f), true);
            _allDevTasks.Add(new DevTask(DevTaskType.Programming, "Project Architecture", 100f), true);
            
            _allEatTasks.Add(new EatTask(EatTaskType.cake, "Nice cake", 100f), true);
            _allEatTasks.Add(new EatTask(EatTaskType.coffee, "Black coffee", 100f), true);
            
            _allDevTaskTypes.Add(DevTaskType.Programming, true);
            _allDevTaskTypes.Add(DevTaskType.Art, true);
            _allDevTaskTypes.Add(DevTaskType.GameDesign, true);
            _allDevTaskTypes.Add(DevTaskType.SoundDesign, false);
            
            _allEatTaskTypes.Add(EatTaskType.coffee, true);
            _allEatTaskTypes.Add(EatTaskType.cake, false);
        }

        public List<IDevTask> GetDevTasks()
        {
            var devTasks = new List<IDevTask>();
            
            foreach (var devTask in _allDevTasks.Keys)
            {
                if (_allDevTasks[devTask])
                {
                    devTasks.Add(devTask);
                }
            }
            
            return devTasks;
        }

        public List<IEatTask> GetEatTasks()
        {
            var eatTasks = new List<IEatTask>();
            
            foreach (var eatTask in _allEatTasks.Keys)
            {
                if (_allEatTasks[eatTask])
                {
                    eatTasks.Add(eatTask);
                }
            }
            
            return eatTasks;
        }

        public List<DevTaskType> GetAvailableDevTasks()
        {
            var devTasks = new List<DevTaskType>();
            
            foreach (var devTask in _allDevTaskTypes.Keys)
            {
                if (_allDevTaskTypes[devTask])
                {
                    devTasks.Add(devTask);
                }
            }
            
            return devTasks;
        }

        public List<EatTaskType> GetAvailableEatTasks()
        {
            var eatTasks = new List<EatTaskType>();

            foreach (var eatTask in _allEatTaskTypes.Keys)
            {
                if (_allEatTaskTypes[eatTask])
                {
                    eatTasks.Add(eatTask);
                }
            }
            
            return eatTasks;
        }

        public void CleanUp()
        {
            _allDevTaskTypes.Clear();
            _allEatTaskTypes.Clear();
            _allDevTasks.Clear();
            _allEatTasks.Clear();
        }
    }
}