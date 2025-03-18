using Core;
using DG.Tweening;
using Scripts.Data;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroLogic : IExecute, ICleanUp
    {
        private readonly HeroStateMachine _heroStateMachine;
        
        public HeroIdleState IdleState { get; private set; }
        public HeroWalkState WalkState { get; private set; }
        public HeroWalkToIOState WalkToIOState { get; private set; }
        public HeroDevState DevState { get; private set; }
        public HeroSleepState SleepState { get; private set; }
        public HeroEatState EatState { get; private set; }

        private readonly HeroConfig _heroConfig;
        private readonly HeroMovementLogic _heroMovementLogic;
        private readonly HeroView _heroView;
        private readonly SpriteRenderer _heroSprite;
        private readonly float _roomSize;
        private readonly ProgressData _progressData;
        private readonly float _yPos;
        private const float Offset = 1f;
        
        private Vector3 _targetPosition;
        private IInteractiveObject _targetIO;
        private Sequence _sequence;

        public HeroLogic(HeroConfig heroConfig, HeroMovementLogic heroMovementLogic, HeroView heroView, Vector3 initialPosition, float roomSize, ProgressData progressData)
        {
            _heroConfig = heroConfig;
            _heroMovementLogic = heroMovementLogic;
            _heroView = heroView;
            _roomSize = roomSize;
            _progressData = progressData;
            _yPos = initialPosition.y;
            _heroSprite = heroView.HeroSprite;
            
            _heroStateMachine = new HeroStateMachine();
            IdleState = new HeroIdleState(this);
            WalkState = new HeroWalkState(this);
            DevState = new HeroDevState(this);
            EatState = new HeroEatState(this);
            SleepState = new HeroSleepState(this);
            WalkToIOState = new HeroWalkToIOState(this);
            
            _heroStateMachine.Init(IdleState);

            _heroMovementLogic.OnGetDestination += MouseListener;
            _heroMovementLogic.OnGetTargetI0 += GetTargetIO;
        }

        public void ChangeStateByIOType(InteractiveObjectType iOType)
        {
            switch (iOType)
            {
                case InteractiveObjectType.None :
                    ChangeState(IdleState);
                    break;
                case InteractiveObjectType.Pc:
                    ChangeState(DevState);
                    break;
                case InteractiveObjectType.Chair:
                    ChangeState(SleepState);
                    break;
                case InteractiveObjectType.Fridge:
                    ChangeState(EatState);
                    break;
                default:
                    ChangeState(IdleState);
                    break;
            }
        }

        public void AnimateHero()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_heroView.transform.DORotate(new Vector3(0,0,180f), 0.4f));
            _sequence.SetLoops(-1);
            _sequence.Play();
        }

        public void StopAnimation()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_heroView.transform.DORotate(new Vector3(0,0,0), 0));
        }

        private void MouseListener(Vector3 pos)
        {
            var roomSize = (_roomSize - Offset)/2;
            
            if (pos.x > -roomSize && pos.x < roomSize)
            {
                _targetPosition = new Vector3(pos.x, _yPos, 0);
            
                _heroStateMachine.ChangeState(WalkState);
                
                FlipHero(_heroView.transform.position.x > _targetPosition.x);
            }
        }

        private void GetTargetIO(IInteractiveObject iO)
        {
            _targetIO = iO;
            _targetPosition = NormalizeVector(iO.Position);
            _heroStateMachine.ChangeState(WalkToIOState);
            FlipHero(_heroView.transform.position.x > _targetPosition.x);
        }

        public Vector3 NormalizeVector(Vector3 vector)
        {
            return new Vector3(vector.x, _yPos, 0);
        }

        private InteractiveObjectType GetTargetType(IInteractiveObject iO)
        {
            return iO.ObjectType;
        }

        private void FlipHero(bool isLeft)
        {
            _heroSprite.flipX = isLeft;
        }

        public void MoveHero(Vector3 from, Vector3 to, float deltaTime)
        {
            Vector3 newPosition = Vector3.MoveTowards(from, to, _heroConfig.WalkSpeed * deltaTime);
            
            _heroView.transform.position = newPosition;
        }
        
        public void Execute(float deltatime)
        {
            _heroStateMachine.CurrentState.Update(deltatime);
        }

        public void CleanUp()
        {
            _heroMovementLogic.OnGetDestination -= MouseListener;
            _heroMovementLogic.OnGetTargetI0 -= GetTargetIO;
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

        public void ChangeProgressData()
        {
            _progressData.ChangeFieldValue(Consts.Food, -0.001f);
            _progressData.ChangeFieldValue(Consts.Energy, -0.0005f);
        }
    }
}