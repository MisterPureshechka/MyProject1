using System;
using System.Threading.Tasks;
using _root.Scripts.Rooms.RoomItems;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public sealed class Employee
    {
        private const float EnergyDrainPerSecond = 0.9f;
        private const float HungerDrainPerSecond = 0.5f;
        private const float MoodDrainPerSecond   = 0.7f;
        //Вынести в конфиг
        private const float MinWorkInterval = 0.5f;  
        private const float MaxWorkInterval = 2.5f;  
        
        private float _curvePower = 2.0f;
        
        private float _workTimer;
        
        public string Id { get; }
        public string Name { get; }

        public float Energy { get; private set; }
        public float Hunger { get; private set; }
        public float Mood { get; private set; }
        
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

        private float CalculateWorkInterval()
        {
            float normalized = Mathf.Clamp01((Energy + Hunger + Mood) / (3f * MaxValue));

            // делаем кривую: низкие значения становятся ещё ниже
            float curved = Mathf.Pow(normalized, _curvePower);

            return Mathf.Lerp(MaxWorkInterval, MinWorkInterval, curved);
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
    }

    public enum EmployeeState
    {
        Work,
        Interacting,
        Wait
    }
}
