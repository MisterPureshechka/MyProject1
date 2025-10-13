using System.Collections.Generic;
using Core;
using DG.Tweening;
using Scripts.Animator;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Job;
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
        private readonly GameProgress _gameProgress;

        private readonly HeroConfig _heroConfig;
        private readonly HeroMovementLogic _heroMovementLogic;
        private readonly HeroStateMachine _heroStateMachine;
        private readonly HeroView _heroView;
        private readonly InteractiveObjectRegisterer _interactiveObjectRegister;
        private readonly LocalEvents _localEvents;
        private readonly ProgressDataAdapter _progressData;

        private readonly float _roomSize;
        private readonly SpriteAnimator _spriteAnimator;
        private readonly HeroAnimator _heroAnimator;
        private readonly float _yPos;
        private IInteractiveObject _exit;
        
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
        public HeroWakeUpState WakeUpState { get; private set; }

        private bool _isAwait;
        private Sequence _sequence;

        private Dictionary<HeroStateId, HeroBaseState> _stateFromId;
        private IInteractiveObject _targetIO;

        private readonly Vector3 _initialPosition;
        private Vector3 _targetPosition;

        public HeroLogic(HeroConfig heroConfig, HeroAnimator heroAnimator, HeroMovementLogic heroMovementLogic, HeroView heroView,
            Vector3 initialPosition, float roomSize, SpriteAnimator spriteAnimator, ProgressDataAdapter progressData,
            GameProgress gameProgress, LocalEvents localEvents, InteractiveObjectRegisterer interactiveObjectRegister)
        {
            _heroConfig = heroConfig;
            _heroAnimator = heroAnimator;
            _heroMovementLogic = heroMovementLogic;
            _heroView = heroView;
            if (_heroView == null)
            {
                _heroView = Object.FindObjectOfType<HeroView>();
                ;
            }

            _roomSize = roomSize;
            _spriteAnimator = spriteAnimator;
            _progressData = progressData;
            _gameProgress = gameProgress;
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
            WakeUpState = new HeroWakeUpState(this);
            ExitState = new HeroExitState(this, _localEvents);
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
            _localEvents.OnHeroGetRootIO += ChangeStateByIOType;
            _localEvents.OnWalkToIO += WalkToIO;
            _localEvents.OnExitEvent += WalkToExit;
            _localEvents.OnHeroGoToBed += WalkToBed;
        }

        public void CleanUp()
        {
            _heroMovementLogic.OnClickI0 -= GetTargetIO;
            _localEvents.OnClosePanel -= PanelCloseCallback;
            _localEvents.OnOpenPanel -= PanelOpenListener;
            _localEvents.OnMouseClickWorld -= OnCLickWorld;
            _localEvents.OnSprintCreated -= ChangeStateByIOType;
            _localEvents.OnTaskCatalogHide -= TaskCatalogHideListener;
            _localEvents.OnSprintComplete -= SprintCompleteListener;
            _localEvents.OnHeroGetRootIO -= ChangeStateByIOType;

            Object.Destroy(_heroView.gameObject);
        }

        public void Execute(float deltatime)
        {
            _heroStateMachine.CurrentState.Update(deltatime);
        }

        private Vector3 LoadInitPos()
        {
            var meta = _progressData.GetProgressData().Metadata;
            meta.TryGetValue(Consts.InitialPosX, out var data);

            if (data != null)
            {
                return NormalizeVector(new Vector3(data.Value, 0, 0));
            }

            Debug.LogError("InitialPos is null ");
            
            return NormalizeVector(_interactiveObjectRegister.GetRootByIOType(InteractiveObjectType.Door).Position);
        }

        public void SaveInitPos(InteractiveObjectType iOType = InteractiveObjectType.None)
        {
            var meta = _progressData.GetProgressData().Metadata;
            
            meta.TryGetValue(Consts.InitialPosX, out var data);

            if (data != null)
            {
                data.Value = _interactiveObjectRegister.GetRootByIOType(iOType).Position.x;
            }
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
            return HeroStateId.Idle;
        }

        private void SetMeta(string key, float value)
        {
            var meta = _progressData.GetProgressData().Metadata;
            if (meta.TryGetValue(key, out var data))
                data.Value = value;
            else
                meta.Add(key, new Meta.Metadata
                {
                    MetaType = MetaType.System,
                    Value = value, MaxValue = 100,
                    DisplayName = key, Tooltip = "",
                    ProgressDelta = 0
                });
        }

        public void SaveHeroState(HeroBaseState state, int? payload = null)
        {
            SetMeta(Consts.HeroStateKey, (int)IdOf(state));
            if (payload.HasValue)
                SetMeta(Consts.HeroStatePayloadKey, payload.Value);
            _gameProgress.SaveProgress(_progressData.GetProgressData());
        }

        private HeroBaseState LoadLastState()
        {
            BuildStateMap();

            var meta = _progressData.GetProgressData().Metadata;

            if (meta.TryGetValue(Consts.HeroStateKey, out var stateData))
            {
                var id = Mathf.RoundToInt(stateData.Value);
                if (_stateFromId != null && _stateFromId.TryGetValue((HeroStateId)id, out var state))
                {
                    return state;
                }
            }

            return IdleState;
        }

        private void WalkToExit(ExitEvent exitEvent)
        {
            ChangeState(WalkToExitState);
            WalkToExitState.SetEventType(exitEvent);
            _localEvents.TriggerHeroWalkToIO();
        }

        private void WalkToBed()
        {
            ChangeState(WalkToBedState);
            _localEvents.TriggerHeroWalkToSprint();
        }

        private void SprintCratedListener(SprintType obj)
        {
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
            ChangeState(WalkToSprintState);
            _localEvents.TriggerHeroWalkToSprint();
        }

        private void WalkToIO(InteractiveObjectType interactiveObjectType)
        {
            ChangeState(WalkToIOState);
        }

        private void ChangeStateByIOType(SprintType iOType)
        {
            switch (iOType)
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
                default:
                    ChangeState(IdleState);
                    break;
            }
        }

        public void TriggerHeroGetIO(SprintType iOType)
        {
            _localEvents.TriggerHeroGetSprint(iOType);
        }

        private void OnCLickWorld(Vector2 position)
        {
            //_isAwait = false;
        }

        private void MouseListener(Vector3 pos)
        {
            if (_isAwait) return;

            var roomSize = (_roomSize - Offset) / 2;

            if (pos.x > -roomSize && pos.x < roomSize)
            {
                _targetPosition = new Vector3(pos.x, _yPos, 0);

                _heroStateMachine.ChangeState(WalkState);

                FlipHero(_heroView.transform.position.x > _targetPosition.x);
            }
        }

        public void PlayAnimation(HeroAnimationState animationState, bool isLoop)
        {
            _heroAnimator.StartAnimation(animationState, isLoop);
        }

        public void PlayTransitionAnimation(HeroAnimationState from, HeroAnimationState to)
        {
        }

        private void GetTargetIO(IInteractiveObject iO)
        {
            if (_isAwait) return;

            _targetIO = iO;
            _targetPosition = NormalizeVector(iO.Position);
        }

        public Vector3 GetIOPositionByType(InteractiveObjectType iOType)
        {
            var ioPos = NormalizeVector(_interactiveObjectRegister.GetRootByIOType(iOType).Position);

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
            _gameProgress.SaveProgress(_progressData.GetProgressData());
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