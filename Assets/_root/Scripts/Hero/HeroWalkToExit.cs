using System;
using _root.Notification;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroWalkToExit : HeroBaseState
    {
        private readonly LocalEvents _localEvents;
        private IInteractiveObject _targetIO;
        private Vector3 _playerPosition;
        private Vector3 _targetPosition;
        
        private ExitEvent _event;

        public HeroWalkToExit(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.SetWalking(true);
            _targetIO = _heroLogic.GetTargetIO();
            
            _targetPosition = _heroLogic.GetIOPositionByType(InteractiveObjectType.Door);
            
            _heroLogic.FlipHero(_heroLogic.HeroPosition().x > _targetPosition.x);
            _heroLogic.PlayAnimation(HeroAnimationState.Walk, true);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            
            _playerPosition = _heroLogic.HeroPosition();

            _heroLogic.MoveHero(_playerPosition, _targetPosition, deltaTime);

            if (Vector3.Distance(_playerPosition, _targetPosition) < 0.25f)
            {
                _heroLogic.ChangeState(_heroLogic.ExitState);
                _localEvents.TriggerExitEventWhenExit(_event);
            }
        }

        public void SetEventType(ExitEvent exitEvent)
        {
            _event = exitEvent;
        }

        public override void Exit()
        {
            _heroLogic.SetWalking(false);
            base.Exit();
        }
    }
}