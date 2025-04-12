using System;
using System.Collections.Generic;
using Core;

namespace Scripts.Tasks
{
    public class TaskLibrary : ICleanUp
    {
        private readonly Dictionary<ITask, bool> _allTasks = new();
        private readonly Dictionary<System.Enum, bool> _allTaskTypes = new();
        private readonly Dictionary<DevTaskType, List<IDevTask>> _allDevTasks = new();
       

        public TaskLibrary()
        {
            LoadAllAvailableTasks();
            LoadAllDevTasks();
        }

        private void LoadAllDevTasks()
        {
            _allDevTasks.Clear();

            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
            {
                _allDevTasks[type] = new List<IDevTask>();
            }

            _allDevTasks[DevTaskType.Programming].Add(new DevTask(DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(DevTaskType.Programming, "Base Mechanics", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(DevTaskType.Programming, "Project Architecture", 100f));

            _allDevTasks[DevTaskType.Art].Add(new DevTask(DevTaskType.Art, "Character Design", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(DevTaskType.Art, "Environment Art", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(DevTaskType.Art, "UI Assets", 100f));

            _allDevTasks[DevTaskType.GameDesign].Add(new DevTask(DevTaskType.GameDesign, "Core Mechanics", 100f));
            _allDevTasks[DevTaskType.GameDesign].Add(new DevTask(DevTaskType.GameDesign, "Level Design", 100f));

            _allDevTasks[DevTaskType.SoundDesign].Add(new DevTask(DevTaskType.SoundDesign, "Background Music", 100f));
            _allDevTasks[DevTaskType.SoundDesign].Add(new DevTask(DevTaskType.SoundDesign, "SFX", 100f));
        }

        public Dictionary<DevTaskType, List<IDevTask>> GetAlLDevTasks()
        {
            return _allDevTasks;
        }

        private void LoadAllAvailableTasks()
        {
            _allTasks.Add(new DevTask(DevTaskType.Programming, "Saving System", 100f), true);
            _allTasks.Add(new DevTask(DevTaskType.Programming, "Base Mechanics", 100f), true);
            _allTasks.Add(new DevTask(DevTaskType.Programming, "Project Architecture", 100f), true);
            
            _allTasks.Add(new DevTask(DevTaskType.Art, "CharacterDesign", 100f), true);
            
            _allTasks.Add(new EatTask(EatTaskType.cake, "Nice cake", 100f), true);
            _allTasks.Add(new EatTask(EatTaskType.coffee, "Black coffee", 100f), true);
            
            _allTaskTypes.Add(DevTaskType.Programming, true);
            _allTaskTypes.Add(DevTaskType.Art, true);
            _allTaskTypes.Add(DevTaskType.GameDesign, true);
            _allTaskTypes.Add(DevTaskType.SoundDesign, false);
            
            _allTaskTypes.Add(EatTaskType.coffee, true);
            _allTaskTypes.Add(EatTaskType.cake, false);
        }

        public List<ITask> GetTasks<T>() where T : ITask
        {
            var tasks = new List<ITask>();
            
            foreach (var task in _allTasks)
            {
                if (task.Key is T && task.Value)
                {
                    tasks.Add(task.Key);
                }
            }
            
            return tasks;
        }

        public List<TEnum> GetAvailableTaskTypes<TEnum>() where TEnum : System.Enum
        {
            var types = new List<TEnum>();
            
            foreach (var type in _allTaskTypes)
            {
                if (type.Key is TEnum enumValue && type.Value)
                {
                    types.Add(enumValue);
                }
            }
            
            return types;
        }

        public void CleanUp()
        {
            _allTasks.Clear();
            _allTaskTypes.Clear();
        }
    }
}