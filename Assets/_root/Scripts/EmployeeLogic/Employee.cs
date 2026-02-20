using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _root.Scripts.Rooms.RoomItems;
using Scripts.EmployeeLogic.Scripts.EmployeeLogic;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public sealed class Employee : ISkillOwner
    {
        private const float EnergyDrainPerSecond = 0.9f;
        private const float HungerDrainPerSecond = 0.5f;
        private const float MoodDrainPerSecond   = 0.7f;
        
        private const float MinWorkInterval = 0.2f;  
        private const float MaxWorkInterval = 2.5f;  
        
        private float _curvePower = 2.0f;
        
        private float _workTimer;
        
        public string Id { get; }
        public string Name { get; }
        
        public event Action<Employee> OnSkillsChanged;

        public float Energy { get; private set; }
        public float Hunger { get; private set; }
        public float Mood { get; private set; }
        
        private readonly Dictionary<DevTaskType, float> _skills = new();
        public IReadOnlyDictionary<DevTaskType, float> Skills => _skills;
        
        public float MaxValue => 100f;

        public EmployeeState _currentState;
        public event Action OnStatUpdate; 

        public EmployeeItemView View { get; set; }

        private ITask _currentTask;

        private bool _isWorking;
        private bool _isInteracting;
        public bool IsBusy => _isInteracting;

        public Employee(string id, string name)
        {
            Id = id;
            Name = name;
            Energy = 100;
            Hunger = 100;
            Mood = 100;
            
            InitSkills();
        }
        
        public void ImportSkills(Dictionary<string, float> skills)
        {
            if (skills == null) return;

            foreach (var kv in skills)
            {
                if (Enum.TryParse<DevTaskType>(kv.Key, out var type))
                    SetSkill(type, kv.Value);
            }
        }
        
        public Dictionary<string, float> ExportSkills()
        {
            var result = new Dictionary<string, float>(_skills.Count);
            foreach (var kv in _skills)
                result[kv.Key.ToString()] = kv.Value;
            return result;
        }
        
        public void Update(float deltaTime, Action<Employee> onWorkTick)
        {
            if (_currentState != EmployeeState.Work || _isInteracting)
                return;

            ConsumeStats(deltaTime);
            OnStatUpdate?.Invoke();

            _workTimer += deltaTime;

            float interval = CalculateWorkInterval();

            if (_workTimer >= interval)
            {
                _workTimer -= interval;
                onWorkTick?.Invoke(this);
            }
        }

        private static readonly float[] IntervalByLevel = { 2.5f, 1.8f, 1.0f, 0.2f };
        
        private const float T0 = 0.25f;
        private const float T1 = 0.50f;
        private const float T2 = 0.75f;

        
        private static int GetLevel(float value01)
        {
            if (value01 < T0) return 0;
            if (value01 < T1) return 1;
            if (value01 < T2) return 2;
            return 3;
        }

        private float CalculateWorkInterval()
        {
            float e01 = Mathf.Clamp01(Energy / MaxValue);
            float h01 = Mathf.Clamp01(Hunger / MaxValue);
            float m01 = Mathf.Clamp01(Mood / MaxValue);

            float eInt = IntervalByLevel[GetLevel(e01)];
            float hInt = IntervalByLevel[GetLevel(h01)];
            float mInt = IntervalByLevel[GetLevel(m01)];

            float avgInterval = (eInt + hInt + mInt) / 3f;
            return Mathf.Clamp(avgInterval, MinWorkInterval, MaxWorkInterval);
        }
        
        private void ConsumeStats(float deltaTime)
        {
            Energy = Mathf.Max(0, Energy - EnergyDrainPerSecond * deltaTime);
            Hunger = Mathf.Max(0, Hunger - HungerDrainPerSecond * deltaTime);
            Mood   = Mathf.Max(0, Mood   - MoodDrainPerSecond   * deltaTime);

            if (Energy <= 0 || Hunger <= 0 || Mood <= 0)
            {
                Debug.Log($"{Name} слишком устал и больше не может работать.");
                OnBreakWork();
            }
        }
        public async Task InteractWithItemAsync(RoomItem item, Action onComplete = null)
        {
            if (_isInteracting)
            {
                Debug.LogWarning($"{Name} уже взаимодействует с другим объектом.");
                return;
            }

            _isInteracting = true;
            PauseWork(); 
            _currentState = EmployeeState.Interacting;

            var config = item.Config;

            Debug.Log($"{Name} начал взаимодействие с {item.Name}.");

            var delayInSeconds = item.Config.TimeToUpdateEmployeeStat;
            await Task.Delay((int)delayInSeconds * 1000);

            IncreaseStat(config);
            Debug.Log($"{Name} восстановил статы: Energy={Energy}, Hunger={Hunger}, Mood={Mood}.");

            _isInteracting = false;
            _currentState = EmployeeState.Wait; 

            onComplete?.Invoke(); 
        }

        public void PauseWork()
        {
            _currentState = EmployeeState.Wait;
        }

        public void ResumeWork()
        {
            _currentState = EmployeeState.Work;
            _workTimer = 0f; 
        }

        private void IncreaseStat(RoomItemConfig config)
        {
            Energy = Mathf.Clamp(Energy + config.EnergyValue, 0f, MaxValue);
            Hunger = Mathf.Clamp(Hunger + config.FoodValue, 0f, MaxValue);
            Mood   = Mathf.Clamp(Mood   + config.MoodValue, 0f, MaxValue);

            OnStatUpdate?.Invoke();
        }

        public void StopInteracting()
        {
            if (!_isInteracting)
            {
                Debug.LogWarning($"{Name} не взаимодействует ни с одним объектом.");
                return;
            }

            _isInteracting = false;
        }
        
        public void ChangeState(EmployeeState state)
        {
            _currentState = state;
        }

        private void OnBreakWork()
        {
            Debug.Log($"{Name} больше не может продолжать.");
            _isWorking = false;
        }
        
        public float GetSkill(DevTaskType type)
            => _skills.TryGetValue(type, out var v) ? v : 0f;

        public void SetSkill(DevTaskType type, float value)
        {
            _skills[type] = Mathf.Clamp(value, 0f, MaxValue);
            OnSkillsChanged?.Invoke(this);
            OnStatUpdate?.Invoke();
        }

        public void AddSkill(DevTaskType type, float delta)
        {
            _skills.TryGetValue(type, out var current);
            _skills[type] = Mathf.Clamp(current + delta, 0f, MaxValue);
            OnSkillsChanged?.Invoke(this);
            OnStatUpdate?.Invoke();
        }

        private void InitSkills()
        {
            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
                _skills[type] = 0;
        }
    }

    public enum EmployeeState
    {
        Work,
        Interacting,
        Wait
    }
}
