using System;
using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;

namespace Scripts.Tasks
{
    public class TaskLibrary : ICleanUp
    {
        private readonly Dictionary<ITask, bool> _allTasks = new();
        private readonly Dictionary<System.Enum, bool> _allTaskTypes = new();
        private readonly Dictionary<DevTaskType, List<IDevTask>> _allDevTasks = new();
        private readonly Dictionary<EatTaskType, List<IEatTask>> _allEatTasks = new();
        private readonly Dictionary<SprintType, ITask> _autoSprints = new();
        private readonly List<IReadTask> _allReadTasks = new();
        private readonly ProgressDataAdapter _progressDataAdapter;
        private LocalEvents _localEvents;

        public TaskLibrary(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            LoadAllAvailableTasks();
            LoadAllDevTasks();
            LoadAllEatTasks();
            LoadAutoSprintTasks();
            LoadReadSprints();
        }

        private void LoadReadSprints()
        {
            _allReadTasks.Add(new ReadTask(_progressDataAdapter, _localEvents, DevTaskType.Programming,"CleanCode", 100f));
            _allReadTasks.Add(new ReadTask(_progressDataAdapter, _localEvents,DevTaskType.Art,"Art", 100f));
            _allReadTasks.Add(new ReadTask(_progressDataAdapter, _localEvents,DevTaskType.GameDesign,"Good Game Design", 100f));
            _allReadTasks.Add(new ReadTask(_progressDataAdapter, _localEvents,DevTaskType.Marketing,"How to publish game", 100f));
        }

        private void LoadAutoSprintTasks()
        {
            _autoSprints[SprintType.Chill] = new ChillTask(_progressDataAdapter, "Just Chill", 100f);
            _autoSprints[SprintType.Play] = new PlayTask(_progressDataAdapter, "Play", 100f);
            _autoSprints[SprintType.Toilet] = new ToiletTask(_progressDataAdapter, "Is it necessary to watch here?", 100f);
            _autoSprints[SprintType.Shower] = new BathTask(_progressDataAdapter, "Bath", 100f);
            _autoSprints[SprintType.Eat] = new EatTask(_progressDataAdapter, EatTaskType.cake, "Nice cake", 25f);
        }

        private void LoadAllDevTasks()
        {
            _allDevTasks.Clear();

            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
            {
                _allDevTasks[type] = new List<IDevTask>();
            }

            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Saving System", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Base Mechanics", 100f));
            _allDevTasks[DevTaskType.Programming].Add(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Project Architecture", 100f));

            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "Character Design", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "Environment Art", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "UI Assets", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "Character Design", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "Environment Art", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "UI Assets", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "Character Design", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "Environment Art", 100f));
            _allDevTasks[DevTaskType.Art].Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "UI Assets", 100f));

            _allDevTasks[DevTaskType.GameDesign].Add(new DevTask(_progressDataAdapter, DevTaskType.GameDesign, "Core Mechanics", 100f));
            _allDevTasks[DevTaskType.GameDesign].Add(new DevTask(_progressDataAdapter, DevTaskType.GameDesign, "Level Design", 100f));

            _allDevTasks[DevTaskType.SoundDesign].Add(new DevTask(_progressDataAdapter, DevTaskType.SoundDesign, "Background Music", 100f));
            _allDevTasks[DevTaskType.SoundDesign].Add(new DevTask(_progressDataAdapter, DevTaskType.SoundDesign, "SFX", 100f));
            
            _allDevTasks[DevTaskType.Marketing].Add(new DevTask(_progressDataAdapter, DevTaskType.Marketing, "Blog", 100f));
        }

        public Dictionary<DevTaskType, List<IDevTask>> GetAlLDevTasks()
        {
            return _allDevTasks;
        }

        private void LoadAllAvailableTasks()
        {
            _allTasks.Add(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Saving System", 100f), true);
            _allTasks.Add(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Base Mechanics", 100f), true);
            _allTasks.Add(new DevTask(_progressDataAdapter, DevTaskType.Programming, "Project Architecture", 100f), true);
            
            _allTasks.Add(new DevTask(_progressDataAdapter, DevTaskType.Art, "CharacterDesign", 100f), true);
            
            _allTaskTypes.Add(DevTaskType.Programming, true);
            _allTaskTypes.Add(DevTaskType.Art, true);
            _allTaskTypes.Add(DevTaskType.GameDesign, true);
            _allTaskTypes.Add(DevTaskType.SoundDesign, false);
        }

        private void LoadAllEatTasks()
        {
            foreach (EatTaskType type in Enum.GetValues(typeof(EatTaskType)))
            {
                _allEatTasks[type] = new List<IEatTask>();
            }
            
            _allEatTasks[EatTaskType.cake].Add(new EatTask(_progressDataAdapter, EatTaskType.cake, "Nice cake", 100f));
            _allEatTasks[EatTaskType.coffee].Add(new EatTask(_progressDataAdapter, EatTaskType.coffee, "Coffee", 100f));
        }

        public ITask GetRandomEatTask()
        {
            return _allEatTasks[EatTaskType.cake][0];
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

        public List<IReadTask> GetReadTasks()
        {
            return _allReadTasks;
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
        
        public ITask GetAutoTasks(SprintType sprintType) => _autoSprints[sprintType];

        public void CleanUp()
        {
            _allTasks.Clear();
            _allTaskTypes.Clear();
        }

    }
}