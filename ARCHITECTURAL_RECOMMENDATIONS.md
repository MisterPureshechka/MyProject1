# Архитектурные рекомендации для проекта Pendent

## Обзор

Документ содержит архитектурные рекомендации для улучшения структуры проекта Pendent в соответствии с AI Specification. Рекомендации основаны на анализе текущей реализации и лучших практиках разработки игр на Unity.

## Текущая архитектура: Анализ

### Сильные стороны
- ✅ **Четкое разделение ответственности**: Сервисы, контроллеры, данные
- ✅ **Dependency Injection**: Использование Zenject
- ✅ **Event-driven архитектура**: [`LocalEvents`](Assets/_root/Scripts/GlobalStateMachine/LocalEvents.cs)
- ✅ **State Machine**: [`GlobalStateMachine`](Assets/_root/Scripts/StateMachine/) для управления состояниями
- ✅ **ScriptableObject конфигурация**: [`GameData`](Assets/_root/Scripts/Data/GameData.cs)

### Проблемы текущей архитектуры
- ❌ **Отсутствует четкий Core Flow**: Нет централизованного управления игровым циклом
- ❌ **Смешанные ответственности**: Некоторые контроллеры делают слишком много
- ❌ **Слабая интеграция**: Системы не связаны должным образом
- ❌ **Отсутствует Domain Layer**: Бизнес-логика разбросана по сервисам

## Рекомендуемая архитектура

### 1. Слоистая архитектура (Layered Architecture)

```
┌─────────────────────────────────────────┐
│           Presentation Layer            │  ← UI Controllers, Views
├─────────────────────────────────────────┤
│          Application Layer              │  ← Use Cases, Flow Controllers
├─────────────────────────────────────────┤
│            Domain Layer                 │  ← Business Logic, Services
├─────────────────────────────────────────┤
│         Infrastructure Layer            │  ← Save/Load, External APIs
└─────────────────────────────────────────┘
```

#### 1.1 Presentation Layer
**Ответственность**: UI логика, отображение данных, пользовательский ввод

**Структура**:
```
Assets/_root/Scripts/Presentation/
├── Controllers/
│   ├── UI/
│   │   ├── MilestoneProgressController.cs
│   │   ├── ShopController.cs
│   │   └── ReleaseResultController.cs
│   └── Input/
│       └── InputController.cs
├── Views/
│   ├── UI/
│   │   ├── MilestoneProgressView.cs
│   │   └── ShopView.cs
│   └── World/
│       └── EmployeeView.cs
└── Presenters/
    ├── MilestonePresenter.cs
    └── ShopPresenter.cs
```

#### 1.2 Application Layer
**Ответственность**: Управление use cases, оркестрация доменных сервисов

**Структура**:
```
Assets/_root/Scripts/Application/
├── FlowControllers/
│   ├── MilestoneFlowController.cs
│   ├── GameFlowController.cs
│   └── ReleaseFlowController.cs
├── UseCases/
│   ├── CompleteMilestoneUseCase.cs
│   ├── ProcessShopPurchaseUseCase.cs
│   └── ReleaseGameUseCase.cs
└── Commands/
    ├── StartMilestoneCommand.cs
    └── HireEmployeeCommand.cs
```

#### 1.3 Domain Layer
**Ответственность**: Бизнес-логика, доменные модели, сервисы

**Структура**:
```
Assets/_root/Scripts/Domain/
├── Models/
│   ├── Milestone.cs
│   ├── GameProject.cs
│   ├── Employee.cs
│   └── DevTask.cs
├── Services/
│   ├── IMilestoneService.cs
│   ├── IEconomyService.cs
│   ├── IDifficultyScalingService.cs
│   └── IReleaseService.cs
├── Repositories/
│   ├── IGameProgressRepository.cs
│   └── IEmployeeRepository.cs
└── Events/
    ├── MilestoneCompletedEvent.cs
    └── GameReleasedEvent.cs
```

#### 1.4 Infrastructure Layer
**Ответственность**: Внешние зависимости, сохранение данных, Unity API

**Структура**:
```
Assets/_root/Scripts/Infrastructure/
├── Persistence/
│   ├── SaveService.cs
│   ├── GameProgressRepository.cs
│   └── ProgressDataAdapter.cs
├── Unity/
│   ├── UnityTimeService.cs
│   └── UnityEventSystem.cs
└── External/
    └── AnalyticsService.cs
```

### 2. Event-Driven Architecture с Domain Events

#### 2.1 Domain Events System
```csharp
// Base domain event
public abstract class DomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

// Specific domain events
public class MilestoneCompletedEvent : DomainEvent
{
    public Milestone Milestone { get; }
    public int MoneyReward { get; }
    public int SalaryCost { get; }
    public int NetProfit { get; }
}

public class GameReleasedEvent : DomainEvent
{
    public ReleaseResult Result { get; }
    public ReleaseMode Mode { get; }
}

// Event dispatcher
public interface IDomainEventDispatcher
{
    void Dispatch(DomainEvent domainEvent);
    void Register<T>(Action<T> handler) where T : DomainEvent;
}
```

#### 2.2 Event Handlers
```csharp
public class MilestoneCompletedEventHandler
{
    private readonly IEconomyService _economyService;
    private readonly IGameProgressRepository _repository;
    
    public void Handle(MilestoneCompletedEvent @event)
    {
        // Обновление экономики
        _economyService.ProcessMilestoneResult(@event.MoneyReward, @event.SalaryCost);
        
        // Сохранение прогресса
        _repository.UpdateProgress(@event.Milestone);
        
        // Триггер следующих событий
        // TriggerShopPhase(), TriggerNextMilestone(), etc.
    }
}
```

### 3. Command Query Responsibility Segregation (CQRS)

#### 3.1 Commands (Изменение состояния)
```csharp
public interface ICommand<TResult>
{
}

public class CompleteMilestoneCommand : ICommand<MilestoneResult>
{
    public Milestone Milestone { get; }
    public List<DevTask> CompletedTasks { get; }
}

public class HireEmployeeCommand : ICommand<Employee>
{
    public string EmployeeName { get; }
    public Dictionary<SkillType, int> Skills { get; }
    public int Cost { get; }
}
```

#### 3.2 Queries (Чтение состояния)
```csharp
public interface IQuery<TResult>
{
}

public class GetCurrentMilestoneQuery : IQuery<Milestone>
{
    public int GameIndex { get; }
    public ProjectStage Stage { get; }
    public int MilestoneIndex { get; }
}

public class GetAvailableEmployeesQuery : IQuery<List<Employee>>
{
    public int MaxCost { get; }
    public SkillType RequiredSkill { get; }
}
```

### 4. Repository Pattern

#### 4.1 Интерфейсы репозиториев
```csharp
public interface IGameProgressRepository
{
    ProgressData LoadProgress();
    void SaveProgress(ProgressData data);
    void UpdateMilestoneProgress(MilestoneProgressData progress);
    void UpdateEconomyData(EconomyData economy);
}

public interface IEmployeeRepository
{
    List<Employee> GetAllEmployees();
    Employee GetEmployeeById(string id);
    void AddEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void RemoveEmployee(string id);
}
```

### 5. Factory Pattern для сложных объектов

#### 5.1 Milestone Factory
```csharp
public interface IMilestoneFactory
{
    Milestone CreateMilestone(ProjectStage stage, int index, DifficultyParameters difficulty);
    List<DevTask> CreateTasksForMilestone(Milestone milestone);
}

public class MilestoneFactory : IMilestoneFactory
{
    private readonly IDifficultyScalingService _difficultyService;
    private readonly ITaskFactory _taskFactory;
    
    public Milestone CreateMilestone(ProjectStage stage, int index, DifficultyParameters difficulty)
    {
        var config = GetMilestoneConfig(stage, index);
        var adjustedDifficulty = _difficultyService.AdjustDifficulty(difficulty, stage, index);
        
        return new Milestone
        {
            Stage = stage,
            Index = index,
            DaysLimit = Mathf.RoundToInt(config.BaseDaysLimit * adjustedDifficulty.DaysLimitModifier),
            MoneyReward = Mathf.RoundToInt(config.BaseReward * adjustedDifficulty.RewardMultiplier),
            RequiredTasks = _taskFactory.CreateTasksForMilestone(stage, adjustedDifficulty)
        };
    }
}
```

### 6. Strategy Pattern для Release Modes

#### 6.1 Release Strategy
```csharp
public interface IReleaseStrategy
{
    ReleaseResultData CalculateReleaseResult(ReleaseData data);
}

public class PublisherReleaseStrategy : IReleaseStrategy
{
    public ReleaseResultData CalculateReleaseResult(ReleaseData data)
    {
        return new ReleaseResultData
        {
            PublisherCut = Mathf.RoundToInt(data.BaseRevenue * 0.3f),
            NetProfit = data.BaseRevenue - (data.BaseRevenue * 0.3f),
            UnitsSold = Mathf.RoundToInt(data.BaseUnitsSold * 1.2f), // +20% stability
            RiskFactor = 1.0f // No risk
        };
    }
}

public class IndieReleaseStrategy : IReleaseStrategy
{
    public ReleaseResultData CalculateReleaseResult(ReleaseData data)
    {
        var marketingMultiplier = 1.0f + (data.MarketingLevel * 0.1f);
        var riskFactor = Random.Range(0.7f, 1.5f);
        
        return new ReleaseResultData
        {
            PublisherCut = 0,
            NetProfit = Mathf.RoundToInt(data.BaseRevenue * marketingMultiplier * riskFactor),
            UnitsSold = Mathf.RoundToInt(data.BaseUnitsSold * marketingMultiplier * riskFactor),
            RiskFactor = riskFactor
        };
    }
}
```

### 7. Observer Pattern для Progress Tracking

#### 7.1 Progress Observers
```csharp
public interface IProgressObserver
{
    void OnMilestoneProgressChanged(float progress);
    void OnTaskCompleted(DevTask task);
    void OnEmployeeSkillChanged(Employee employee, SkillType skill, int newLevel);
}

public class MilestoneProgressTracker : IProgressObserver
{
    private readonly Milestone _currentMilestone;
    private readonly IDomainEventDispatcher _eventDispatcher;
    
    public void OnTaskCompleted(DevTask task)
    {
        var progress = CalculateMilestoneProgress();
        _eventDispatcher.Dispatch(new MilestoneProgressChangedEvent(progress));
        
        if (progress >= 1.0f)
        {
            _eventDispatcher.Dispatch(new MilestoneCompletedEvent(_currentMilestone));
        }
    }
}
```

## Рекомендации по рефакторингу

### Фаза 1: Создание Domain Layer (1-2 недели)

1. **Создать доменные модели**
   - `Milestone`, `GameProject`, `Employee`, `DevTask`
   - Перенести бизнес-логику из сервисов в модели

2. **Реализовать Repository Pattern**
   - Создать интерфейсы репозиториев
   - Реализовать конкретные классы

3. **Внедрить Domain Events**
   - Создать систему доменных событий
   - Перенести логику из [`LocalEvents`](Assets/_root/Scripts/GlobalStateMachine/LocalEvents.cs) в доменные события

### Фаза 2: Application Layer (1-2 недели)

1. **Создать Flow Controllers**
   - `MilestoneFlowController`
   - `GameFlowController`
   - `ReleaseFlowController`

2. **Реализовать Use Cases**
   - `CompleteMilestoneUseCase`
   - `ProcessShopPurchaseUseCase`
   - `ReleaseGameUseCase`

3. **Внедрить CQRS**
   - Создать команды и запросы
   - Реализовать обработчики

### Фаза 3: Presentation Layer (1 неделя)

1. **Рефакторинг UI контроллеров**
   - Разделить логику и отображение
   - Внедрить Presenter pattern

2. **Оптимизация Event System**
   - Унифицировать события
   - Улучшить производительность

### Фаза 4: Интеграция и тестирование (1-2 недели)

1. **Интеграция всех слоев**
2. **Миграция существующего кода**
3. **Комплексное тестирование**

## Конкретные рекомендации по коду

### 1. Избавиться от God Objects
```csharp
// Плохо: LocalEvents делает слишком много
public class LocalEvents : IController
{
    // 300+ строк различных событий
}

// Хорошо: Специализированные диспетчеры
public class GameEventDispatcher : IDomainEventDispatcher { }
public class UIEventDispatcher : IUIEventDispatcher { }
public class InputEventDispatcher : IInputEventDispatcher { }
```

### 2. Принцип Single Responsibility
```csharp
// Плохо: Company делает слишком много
public class Company : IController
{
    // Управление сотрудниками
    // Сохранение прогресса
    // Обработка экономики
    // UI логика
}

// Хорошо: Разделение ответственности
public class EmployeeService : IEmployeeService { }
public class CompanyEconomyService : ICompanyEconomyService { }
public class EmployeeRepository : IEmployeeRepository { }
```

### 3. Dependency Inversion Principle
```csharp
// Плохо: Зависимость от конкретных реализаций
public class MilestoneService
{
    private readonly SaveService _saveService;
    private readonly Company _company;
}

// Хорошо: Зависимость от абстракций
public class MilestoneService
{
    private readonly IGameProgressRepository _repository;
    private readonly IEmployeeService _employeeService;
}
```

## Производительность и оптимизация

### 1. Object Pooling для UI элементов
```csharp
public class UIElementPool<T> where T : Component
{
    private readonly Queue<T> _pool = new Queue<T>();
    private readonly Func<T> _createFunc;
    
    public T Get()
    {
        return _pool.Count > 0 ? _pool.Dequeue() : _createFunc();
    }
    
    public void Return(T item)
    {
        item.gameObject.SetActive(false);
        _pool.Enqueue(item);
    }
}
```

### 2. Lazy Loading для тяжелых ресурсов
```csharp
public class LazyGameData
{
    private GameData _gameData;
    private readonly object _lock = new object();
    
    public GameData Value
    {
        get
        {
            if (_gameData == null)
            {
                lock (_lock)
                {
                    if (_gameData == null)
                    {
                        _gameData = Resources.Load<GameData>("GameData");
                    }
                }
            }
            return _gameData;
        }
    }
}
```

### 3. Event Optimization
```csharp
// Использование struct для простых событий
public readonly struct MilestoneProgressChangedEvent
{
    public readonly float Progress;
    public readonly int MilestoneIndex;
    
    public MilestoneProgressChangedEvent(float progress, int milestoneIndex)
    {
        Progress = progress;
        MilestoneIndex = milestoneIndex;
    }
}
```

## Тестирование

### 1. Unit Tests для Domain Layer
```csharp
[Test]
public void Milestone_Completion_WithAllTasks_ReturnsCompleted()
{
    // Arrange
    var milestone = new Milestone();
    var tasks = new List<DevTask> { /* completed tasks */ };
    
    // Act
    var result = milestone.CheckCompletion(tasks);
    
    // Assert
    Assert.IsTrue(result.IsCompleted);
}
```

### 2. Integration Tests для Application Layer
```csharp
[Test]
public void CompleteMilestoneUseCase_WithValidData_UpdatesProgress()
{
    // Arrange
    var useCase = new CompleteMilestoneUseCase(repository, economyService);
    var command = new CompleteMilestoneCommand { /* data */ };
    
    // Act
    var result = useCase.Execute(command);
    
    // Assert
    Assert.IsNotNull(result);
    // Verify repository calls
}
```

## Заключение

Предложенная архитектура обеспечит:

- **Масштабируемость**: Легкое добавление новых функций
- **Тестируемость**: Изолированные компоненты легко тестировать
- **Поддерживаемость**: Четкое разделение ответственности
- **Производительность**: Оптимизированные паттерны и практики

Рефакторинг следует проводить поэтапно, начиная с Domain Layer, чтобы минимизировать риски и обеспечить плавный переход.