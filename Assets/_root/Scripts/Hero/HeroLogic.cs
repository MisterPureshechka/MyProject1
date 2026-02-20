using System.Collections.Generic;
using Core;
using DG.Tweening;
using Scripts.Animator;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Hero
{
    public class HeroLogic : IExecute, ICleanUp
    {
        private const float Offset = 1f;
        private readonly SaveService _saveService;

        private readonly HeroConfig _heroConfig;
        private readonly HeroMovementLogic _heroMovementLogic;
        private readonly HeroStateMachine _heroStateMachine;
        private readonly HeroView _heroView;
        private readonly InteractiveObjectRegisterer _interactiveObjectRegister;
        private readonly LocalEvents _localEvents;
        private readonly ProgressDataAdapterOLD _progressData;

        private readonly float _roomSize;
        private readonly SpriteAnimator _spriteAnimator;
        private readonly HeroAnimator _heroAnimator;
        private readonly float _yPos;
        private IInteractiveObject _exit;
        
        private const string EnergyKey = "Energy";
        private const string FoodKey   = "Food";
        private const string MoodKey   = "Mood";

        private const float MIN_ENERGY = 0.15f;
        private const float MIN_FOOD   = 0.15f;
        private const float MIN_MOOD   = 0.20f;
        
        public HeroIdleState IdleState { get; }
        public HeroWalkState WalkState { get; }
        public HeroWalkToSprint WalkToSprintState { get; }
        public HeroWalkToIO WalkToIOState { get; }
        public HeroWalkToExit WalkToExitState { get; }
        public HeroExitState ExitState { get; }
        public HeroEnterState EnterState { get; }
        public HeroWalkToRootIOState WalkToRootIOState { get; }
        public HeroWalkToBedState WalkToBedState { get; }
        public HeroDevState DevState { get; }
        public HeroSleepState SleepState { get; }
        public HeroEatState EatState { get; }
        public HeroPlayState PlayState { get; }
        public HeroReadState ReadState { get; }
        public HeroChillState ChillState { get; }
        public HeroAwaitState HeroAwaitState { get; }
        public HeroToiletState HeroToiletState { get; }
        public HeroBathState HeroBathState { get; }
        public HeroCleanState CleanState { get; }
        public HeroWakeUpState WakeUpState { get; private set; }

        private bool _isAwait;
        private Sequence _sequence;

        private Dictionary<HeroStateId, HeroBaseState> _stateFromId;
        private IInteractiveObject _targetIO;

        private readonly Vector3 _initialPosition;
        private Vector3 _targetPosition;
        private bool _isWalking;
        
        private Dictionary<SprintType, IInteractiveObject> _sprintToIOMap;
        private SprintType _lastRequestedSprintType = SprintType.None;
        

        public HeroLogic(HeroConfig heroConfig, HeroAnimator heroAnimator, HeroMovementLogic heroMovementLogic, HeroView heroView,
            Vector3 initialPosition, float roomSize, SpriteAnimator spriteAnimator, ProgressDataAdapterOLD progressData,
            SaveService saveService, LocalEvents localEvents, InteractiveObjectRegisterer interactiveObjectRegister)
        {
            _heroConfig = heroConfig;
            _heroAnimator = heroAnimator;
            _heroMovementLogic = heroMovementLogic;
            _heroView = heroView;
            if (_heroView == null)
            {
                _heroView = Object.FindObjectOfType<HeroView>();
            }

            _roomSize = roomSize;
            _spriteAnimator = spriteAnimator;
            _progressData = progressData;
            _saveService = saveService;
            _localEvents = localEvents;
            _interactiveObjectRegister = interactiveObjectRegister;
            _yPos = initialPosition.y;
            _initialPosition = LoadInitPos();
            _heroView.transform.position = _initialPosition;
            

            _heroStateMachine = new HeroStateMachine();
            IdleState = new HeroIdleState(this);
            WalkState = new HeroWalkState(this);
            DevState = new HeroDevState(this, _progressData, _localEvents);
            EatState = new HeroEatState(this, _progressData);
            SleepState = new HeroSleepState(this, _localEvents);
            WalkToSprintState = new HeroWalkToSprint(this, _localEvents);
            WalkToRootIOState = new HeroWalkToRootIOState(this, _localEvents);
            ReadState = new HeroReadState(this, _progressData);
            ChillState = new HeroChillState(this, _progressData, _localEvents);
            PlayState = new HeroPlayState(this, _progressData);
            HeroAwaitState = new HeroAwaitState(this);
            HeroToiletState = new HeroToiletState(this);
            HeroBathState = new HeroBathState(this);
            WalkToIOState = new HeroWalkToIO(this, _localEvents);
            WalkToExitState = new HeroWalkToExit(this, _localEvents);
            SleepState = new HeroSleepState(this, _localEvents);
            WalkToBedState = new HeroWalkToBedState(this);
            WakeUpState = new HeroWakeUpState(this, _localEvents);
            ExitState = new HeroExitState(this, _localEvents);
            CleanState = new HeroCleanState(this);
            EnterState = new HeroEnterState(this, _localEvents);

            _heroStateMachine.Init(LoadLastState());

            _heroMovementLogic.OnClickI0 += GetTargetIO;
            _localEvents.OnClosePanel += PanelCloseCallback;
            _localEvents.OnOpenPanel += PanelOpenListener;
            _localEvents.OnTaskCatalogHide += TaskCatalogHideListener;
            //_localEvents.OnSprintCreated += ChangeStateByIOType;
            _localEvents.OnSprintCreated += SprintCratedListener;
            _localEvents.OnWalkToSprint += WalkToSprint;
            _localEvents.OnSprintComplete += SprintCompleteListener;
            _localEvents.OnHeroGetRootIO += ChangeStateBySprintType;
            _localEvents.OnWalkToIO += WalkToIO;
            _localEvents.OnHeroGoToBed += WalkToBed;
            _localEvents.OnTakeCoffee += TakeCoffee;
            
            _sprintToIOMap = new Dictionary<SprintType, IInteractiveObject>
            {
                [SprintType.Dev]    = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Pc),
                [SprintType.Play]   = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.TV),
                [SprintType.Eat]    = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Fridge),
                [SprintType.Shower] = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Bath),
                [SprintType.Read]   = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Books),
                [SprintType.Toilet] = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Toilet),
                [SprintType.Chill]  = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Sofa),
                [SprintType.CleanPc] = _interactiveObjectRegister.GetIOByType(InteractiveObjectType.Pc),
            };
        }
        
        private void TakeCoffee(bool hasCoffee)
        {
            if (hasCoffee)
            {
                ChangeState(IdleState);
            }
        }

        public void CleanUp()
        {
            _heroMovementLogic.OnClickI0 -= GetTargetIO;
            _localEvents.OnClosePanel -= PanelCloseCallback;
            _localEvents.OnOpenPanel -= PanelOpenListener;
            _localEvents.OnSprintCreated -= ChangeStateBySprintType;
            _localEvents.OnTaskCatalogHide -= TaskCatalogHideListener;
            _localEvents.OnSprintComplete -= SprintCompleteListener;
            _localEvents.OnHeroGetRootIO -= ChangeStateBySprintType;
            _localEvents.OnTakeCoffee -= TakeCoffee;

            Object.Destroy(_heroView.gameObject);
        }

        public void Execute(float deltatime)
        {
            _heroStateMachine.CurrentState.Update(deltatime);
        }

        private Vector3 LoadInitPos()
        {
            // var meta = _progressData.GetProgressData().Metadata;
            // meta.TryGetValue(Consts.InitialPosX, out var data);
            //
            // if (data != null)
            // {
            //     return NormalizeVector(new Vector3(data.Value, 0, 0));
            // }
            //
            // Debug.LogError("InitialPos is null!");
            //
            return NormalizeVector(_interactiveObjectRegister.GetIOByType(InteractiveObjectType.Door).Position);
        }

        public void SaveInitPos(InteractiveObjectType iOType = InteractiveObjectType.None)
        {
            // var meta = _progressData.GetProgressData().Metadata;
            //
            // meta.TryGetValue(Consts.InitialPosX, out var data);
            //
            // if (data != null)
            // {
            //     data.Value = _interactiveObjectRegister.GetIOByType(iOType).Position.x;
            // }
        }

        private void BuildStateMap()
        {
            _stateFromId = new Dictionary<HeroStateId, HeroBaseState>
            {
                [HeroStateId.Idle] = IdleState,
                [HeroStateId.Walk] = WalkState,
                [HeroStateId.WalkToSprint] = WalkToSprintState,
                [HeroStateId.WalkToIO] = WalkToIOState,
                [HeroStateId.WalkToExit] = WalkToExitState,
                [HeroStateId.WalkToRootIO] = WalkToRootIOState,
                [HeroStateId.WalkToBed] = WalkToBedState,
                [HeroStateId.Dev] = DevState,
                [HeroStateId.Eat] = EatState,
                [HeroStateId.Sleep] = SleepState,
                [HeroStateId.Play] = PlayState,
                [HeroStateId.Read] = ReadState,
                [HeroStateId.Chill] = ChillState,
                [HeroStateId.Await] = HeroAwaitState,
                [HeroStateId.Toilet] = HeroToiletState,
                [HeroStateId.Bath] = HeroBathState,
                [HeroStateId.WakeUp] = WakeUpState,
                [HeroStateId.Exit] = ExitState,
                [HeroStateId.Enter] = EnterState,
                [HeroStateId.Clean] = CleanState,
            };
        }

        private HeroStateId IdOf(HeroBaseState state)
        {
            if (state == IdleState) return HeroStateId.Idle;
            if (state == WalkState) return HeroStateId.Walk;
            if (state == WalkToSprintState) return HeroStateId.WalkToSprint;
            if (state == WalkToIOState) return HeroStateId.WalkToIO;
            if (state == WalkToExitState) return HeroStateId.WalkToExit;
            if (state == WalkToRootIOState) return HeroStateId.WalkToRootIO;
            if (state == WalkToBedState) return HeroStateId.WalkToBed;
            if (state == DevState) return HeroStateId.Dev;
            if (state == EatState) return HeroStateId.Eat;
            if (state == SleepState) return HeroStateId.Sleep;
            if (state == PlayState) return HeroStateId.Play;
            if (state == ReadState) return HeroStateId.Read;
            if (state == ChillState) return HeroStateId.Chill;
            if (state == HeroAwaitState) return HeroStateId.Await;
            if (state == HeroToiletState) return HeroStateId.Toilet;
            if (state == HeroBathState) return HeroStateId.Bath;
            if (state == WakeUpState) return HeroStateId.WakeUp;
            if (state == ExitState) return HeroStateId.Exit;
            if (state == EnterState) return HeroStateId.Enter;
            if(state == CleanState) return HeroStateId.Clean;
            return HeroStateId.Idle;
        }


        private void SetMeta(string key, float value)
        {
            // var meta = _progressData.GetProgressData().Metadata;
            // if (meta.TryGetValue(key, out var data))
            //     data.Value = value;
            // else
            //     meta.Add(key, new Meta.Metadata
            //     {
            //         MetaType = MetaType.System,
            //         Value = value, MaxValue = 100,
            //         DisplayName = key, Tooltip = "",
            //         ProgressDelta = 0
            //     });
        }

        public void SaveHeroState(HeroBaseState state, int? payload = null)
        {
            SetMeta(Consts.HeroStateKey, (int)IdOf(state));
            if (payload.HasValue)
                SetMeta(Consts.HeroStatePayloadKey, payload.Value);
            _saveService.SaveProgress(_progressData.GetProgressData());
        }

        private HeroBaseState LoadLastState()
        {
            // BuildStateMap();
            //
            // var meta = _progressData.GetProgressData().Metadata;
            //
            // if (meta.TryGetValue(Consts.HeroStateKey, out var stateData))
            // {
            //     var id = Mathf.RoundToInt(stateData.Value);
            //     if (_stateFromId != null && _stateFromId.TryGetValue((HeroStateId)id, out var state))
            //     {
            //         return state;
            //     }
            // }

            return IdleState;
        }

        

        private void WalkToBed()
        {
            ChangeState(WalkToBedState);
            _localEvents.TriggerHeroWalkToSprint();
        }

        private void SprintCratedListener(SprintType obj)
        {
            WalkToRootIOState.SetDesiredSprintType(_lastRequestedSprintType); 
            ChangeState(WalkToRootIOState);
        }

        private void SprintCompleteListener(SprintType type)
        {
            ChangeState(IdleState);
        }

        private void TaskCatalogHideListener(SprintType obj)
        {
            ChangeState(IdleState);
        }

        private void WalkToSprint(SprintType sprintType)
        {
            if (_sprintToIOMap.TryGetValue(sprintType, out var io))
            {
                _targetIO = io;
                _targetPosition = NormalizeVector(io.Position);

                _lastRequestedSprintType = sprintType;               
                WalkToSprintState.SetDesiredSprintType(sprintType);   
                WalkToRootIOState.SetDesiredSprintType(sprintType);
            }
            else return;
            
            if (!CheckStat(sprintType))
            {
                _localEvents.TriggerBlockSprint();
                if (_isWalking)
                {
                    ChangeState(IdleState);
                }
                return;
            }
            
            _localEvents.TriggerHeroWalkToSprint();
            ChangeState(WalkToSprintState);
        }

        public void SetWalking(bool isWalking)
        {
            _localEvents.TriggerHeroWalking(isWalking);
            _isWalking = isWalking;
        }

        private bool CheckStat(SprintType sprintType)
        {
            bool Below(string key, float minNorm, out float normalized)
            {
                var meta = _progressData.GetMetadata(key);
                if (meta == null) { normalized = 1f; return false; }
                float max = Mathf.Max(1f, meta.MaxValue);
                normalized = Mathf.Clamp01(meta.Value / max);
                return normalized < minNorm;
            }

            if (Below(EnergyKey, MIN_ENERGY, out var eNorm))
            {
                switch (sprintType)
                {
                    case SprintType.Chill:
                    case SprintType.Eat:
                    case SprintType.Shower:
                    case SprintType.Toilet:
                        break; 
                    default:
                        _localEvents.TriggerNotEnoughEnergy(); 
                        return false;
                }
            }

            if (Below(FoodKey, MIN_FOOD, out var fNorm))
            {
                if (sprintType != SprintType.Eat) 
                {
                    _localEvents.TriggerNotEnoughFood();
                    return false;
                }
            }

            if (Below(MoodKey, MIN_MOOD, out var mNorm))
            {
                switch (sprintType)
                {
                    case SprintType.Dev:
                    case SprintType.Read:
                        _localEvents.TriggerNotEnoughMood();
                        return false;
                    default:
                        // Chill/Shower/Eat/Toilet — разрешаем
                        break;
                }
            }

            return true;
        }

        private void WalkToIO(InteractiveObjectType interactiveObjectType)
        {
            ChangeState(WalkToIOState);
        }

        private void ChangeStateBySprintType(SprintType sprintType)
        {
            switch (sprintType)
            {
                case SprintType.None:
                    ChangeState(IdleState);
                    break;
                case SprintType.Dev:
                    ChangeState(DevState);
                    break;
                case SprintType.Eat:
                    ChangeState(EatState);
                    break;
                case SprintType.Read:
                    ChangeState(ReadState);
                    break;
                case SprintType.Play:
                    ChangeState(PlayState);
                    break;
                case SprintType.Chill:
                    ChangeState(ChillState);
                    break;
                case SprintType.Toilet:
                    ChangeState(HeroToiletState);
                    break;
                case SprintType.Shower:
                    ChangeState(HeroBathState);
                    break;
                case SprintType.CleanPc:
                    ChangeState(CleanState);
                    break;
                default:
                    ChangeState(IdleState);
                    break;
            }
        }

        public void PlayAnimation(HeroAnimationState animationState, bool isLoop)
        {
            _heroAnimator.StartAnimation(animationState, isLoop);
        }

        private void GetTargetIO(IInteractiveObject iO)
        {
            //if (_isAwait) return;

            _targetIO = iO;
            _targetPosition = NormalizeVector(iO.Position);
        }

        public Vector3 GetIOPositionByType(InteractiveObjectType iOType)
        {
            var ioPos = NormalizeVector(_interactiveObjectRegister.GetIOByType(iOType).Position);

            return ioPos;
        }

        private void PanelCloseCallback()
        {
            _isAwait = false;
        }

        private void PanelOpenListener()
        {
            _isAwait = true;
        }

        public Vector3 NormalizeVector(Vector3 vector)
        {
            return new Vector3(vector.x, _yPos, 0);
        }

        public void ChangeSortingOrder(int sortingOrder)
        {
            _heroView.SetSortingOrder(sortingOrder);
        }

        private SprintType GetTargetType(IInteractiveObject iO)
        {
            return iO.SprintType;
        }

        public void FlipHero(bool isLeft)
        {
            _heroView.FlipX(isLeft);
        }

        public void MoveHero(Vector3 from, Vector3 to, float deltaTime)
        {
            var newPosition = Vector3.MoveTowards(from, to, _heroConfig.WalkSpeed * deltaTime);

            _heroView.transform.position = newPosition;
        }

        public Vector3 HeroPosition()
        {
            return _heroView.transform.position;
        }

        public void ChangeState(HeroBaseState state)
        {
            _heroStateMachine.ChangeState(state);
        }

        public Vector3 GetTargetPosition()
        {
            return _targetPosition;
        }

        public IInteractiveObject GetTargetIO()
        {
            return _targetIO;
        }

        public void SaveProgress()
        {
            _saveService.SaveProgress(_progressData.GetProgressData());
        }

        public void PlaceHero(Vector3 targetPosition)
        {
            _heroView.Transform.position = targetPosition;
        }

        public void ResetHeroPosition()
        {
            var position = _heroView.transform.position;
            _heroView.transform.position = new Vector3(position.x, _yPos, position.z);
        }

        public void TiggerSprintExit()
        {
            _localEvents.TriggerSprintExit();
        }
    }
}