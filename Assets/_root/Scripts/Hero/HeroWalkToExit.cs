using System;
using _root.Notification;
using Scripts.GlobalStateMachine;
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
        
        private CalendarEventType _eventType;

        public HeroWalkToExit(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _targetIO = _heroLogic.GetTargetIO();
            
            _targetPosition = _heroLogic.NormalizeVector(_targetIO.Position);
            
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
                _heroLogic.ChangeState(_heroLogic.HeroAwaitState);
                _localEvents.TriggerEventOnExit(_eventType);
                _localEvents.TriggerHeroGetExit();
            }
        }

        public void SetEventType(CalendarEventType eventType)
        {
            _eventType = eventType;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}