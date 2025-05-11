using System;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using NUnit.Framework;
using Scripts.Animator;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroLogic : IExecute, ICleanUp
    {
        public Action OnDevActiveStat;
        public Action OnChillActiveStat;
        
        private readonly HeroStateMachine _heroStateMachine;
        
        public HeroIdleState IdleState { get; private set; }
        public HeroWalkState WalkState { get; private set; }
        public HeroWalkToIOState WalkToIOState { get; private set; }
        public HeroDevState DevState { get; private set; }
        public HeroSleepState SleepState { get; private set; }
        public HeroEatState EatState { get; private set; }
        public HeroPlayState PlayState { get; private set; }
        public HeroReadState ReadState { get; private set; }
        public HeroChillState ChillState { get; private set; }
        public HeroAwaitState HeroAwaitState { get; private set; }

        private readonly HeroConfig _heroConfig;
        private readonly HeroMovementLogic _heroMovementLogic;
        private readonly HeroView _heroView;
        private readonly SpriteRenderer _heroSprite;
        private readonly SpriteAnimator _spriteAnimator;
        private readonly ProgressDataAdapter _progressData;
        private readonly GameProgress _gameProgress;
        private readonly LocalEvents _localEvents;

        private readonly float _roomSize;
        private readonly float _yPos;

        private bool _isAwait;
        
        private const float Offset = 1f;
        
        private Vector3 _targetPosition;
        private IInteractiveObject _targetIO;
        private Sequence _sequence;

        public HeroLogic(HeroConfig heroConfig, HeroMovementLogic heroMovementLogic, HeroView heroView,
            Vector3 initialPosition, float roomSize, SpriteAnimator spriteAnimator, ProgressDataAdapter progressData,
            GameProgress gameProgress, LocalEvents localEvents)
        {
            _heroConfig = heroConfig;
            _heroMovementLogic = heroMovementLogic;
            _heroView = heroView;
            _roomSize = roomSize;
            _spriteAnimator = spriteAnimator;
            _progressData = progressData;
            _gameProgress = gameProgress;
            _localEvents = localEvents;
            _yPos = initialPosition.y;
            _heroSprite = heroView.HeroSprite;
            
            _heroStateMachine = new HeroStateMachine();
            IdleState = new HeroIdleState(this);
            WalkState = new HeroWalkState(this);
            DevState = new HeroDevState(this, _progressData, _localEvents);
            EatState = new HeroEatState(this, _progressData);
            SleepState = new HeroSleepState(this, _progressData);
            WalkToIOState = new HeroWalkToIOState(this, _localEvents);
            ReadState = new HeroReadState(this, _progressData);
            ChillState = new HeroChillState(this, _progressData, _localEvents);
            PlayState = new HeroPlayState(this, _progressData);
            HeroAwaitState = new HeroAwaitState(this);
            
            _heroStateMachine.Init(IdleState);

            _heroMovementLogic.OnClickDestination += MouseListener;
            _heroMovementLogic.OnClickI0 += GetTargetIO;
            _localEvents.OnClosePanel += PanelCloseCallback;
            _localEvents.OnOpenPanel += PanelOpenListener;
            _localEvents.OnTaskCatalogHide += TaskCatalogHideListener;
            _localEvents.OnSprintCreated += ChangeStateByIOType;
            _localEvents.OnWalkToIO += WalkToIO;
            _localEvents.OnSprintComplete += SprintCompleteListener;
        }

        private void SprintCompleteListener(SprintType type)
        {
            Debug.Log("Sprint complete - " + type);
            ChangeState(IdleState);
        }

        private void TaskCatalogHideListener(SprintType obj)
        {
            ChangeState(IdleState);
        }

        private void WalkToIO(SprintType sprintType)
        {
            ChangeState(WalkToIOState);
            _localEvents.TriggerHeroWalkToIO();
        }

        private void ChangeStateByIOType(SprintType iOType)
        {
            switch (iOType)
            {
                case SprintType.None :
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
                default:
                    ChangeState(IdleState);
                    break;
            }
        }
        
        public void TriggerHeroGetIO(SprintType iOType)
        {
            _localEvents.TriggerHeroGetIO(iOType);
        }

        private void OnCLickWorld(Vector2 position)
        {
            _isAwait = false;
        }

        private void HeroAwaitListener(bool isAwait)
        {
            _isAwait = isAwait;
        }

        public void TriggerActiveSprintByType(SprintType sprintType)
        {
            _localEvents.TriggerActiveSprintByType(sprintType);
        }

        private void ClosedSprintListener(SprintType sprintType)
        {
            ChangeState(IdleState);
        }

        private void MouseListener(Vector3 pos)
        {
            if(_isAwait) return;
            
            var roomSize = (_roomSize - Offset)/2;
            
            if (pos.x > -roomSize && pos.x < roomSize)
            {
                _targetPosition = new Vector3(pos.x, _yPos, 0);
            
                _heroStateMachine.ChangeState(WalkState);
                
                FlipHero(_heroView.transform.position.x > _targetPosition.x);
            }
        }

        public void PlayAnimation(HeroAnimationState animationState, bool isLoop)
        {
            var sequence = _heroConfig.Sequences.Find(s => s.HeroAnimationState == animationState);
            if (sequence != null)
                _spriteAnimator.StartAnimation(_heroSprite, sequence?.Sprites, isLoop, sequence.Speed);
        }

        public void PlayTransitionAnimation(HeroAnimationState from, HeroAnimationState to)
        {
            var fromSequence = _heroConfig.Sequences.Find(f => f.HeroAnimationState == from);   
            var toSequence = _heroConfig.Sequences.Find(t => t.HeroAnimationState == to);
            
            _spriteAnimator.StartAnimation(_heroSprite, fromSequence.Sprites, false, toSequence.Speed, () =>
            {
                _spriteAnimator.StartAnimation(_heroSprite, toSequence.Sprites, true, toSequence.Speed);
            });
        }

        private void GetTargetIO(IInteractiveObject iO)
        {
            if(_isAwait) return;
            
            _targetIO = iO;
            _targetPosition = NormalizeVector(iO.Position);
            // _heroStateMachine.ChangeState(WalkToIOState);
            // FlipHero(_heroView.transform.position.x > _targetPosition.x);
        }

        private void PanelCloseCallback()
        {
            _isAwait = false;
            //ChangeState(IdleState);
        }

        private void PanelOpenListener()
        {
            _isAwait = true;
            /////ChangeState(HeroAwaitState);
        }

        public Vector3 NormalizeVector(Vector3 vector)
        {
            return new Vector3(vector.x, _yPos, 0);
        }

        public void ChangeSortingOrder(int sortingOrder)
        {
            _heroSprite.sortingOrder = sortingOrder;
        }

        private SprintType GetTargetType(IInteractiveObject iO)
        {
            return iO.SprintType;
        }

        public void FlipHero(bool isLeft)
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
            _heroView.transform.position = targetPosition;
        }

        public void CleanUp()
        {
            _heroMovementLogic.OnClickDestination -= MouseListener;
            _heroMovementLogic.OnClickI0 -= GetTargetIO;
            _localEvents.OnClosePanel -= PanelCloseCallback;
            _localEvents.OnOpenPanel -= PanelOpenListener;
            _localEvents.OnMouseClickWorld -= OnCLickWorld;
            _localEvents.OnSprintCreated -= ChangeStateByIOType;
            _localEvents.OnTaskCatalogHide -= TaskCatalogHideListener;
            _localEvents.OnSprintComplete -= SprintCompleteListener;
        }

        public void TiggerSprintExit()
        {
            _localEvents.TriggerSprintExit();
        }
    }
}