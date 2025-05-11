using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroWalkToIOState : HeroBaseState
    {
        private readonly LocalEvents _localEvents;
        private IInteractiveObject _targetIO;
        private Vector3 _playerPosition;
        private Vector3 _targetPosition;

        public HeroWalkToIOState(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _targetIO = _heroLogic.GetTargetIO();
            
            if (_targetIO.RootObjectPosition != null)
            {
                _targetPosition = _heroLogic.NormalizeVector(_targetIO.RootObjectPosition.position);
            }
            else
            {
                _targetPosition = _heroLogic.NormalizeVector(_targetIO.Position);
            }
            
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
                _heroLogic.PlaceHero(_heroLogic.NormalizeVector(_targetPosition));
                //_heroLogic.ChangeStateByIOType(_targetIO.SprintType);
                _localEvents.TriggerHeroGetIO(_targetIO.SprintType);
                //_heroLogic.ChangeState(_heroLogic.HeroAwaitState);
                // _heroLogic.TriggerIOBySprintType(_targetIO.SprintType);
            }
        }

        public override void Exit()
        {
        }
    }
}