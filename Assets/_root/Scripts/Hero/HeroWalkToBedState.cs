using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroWalkToBedState : HeroBaseState
    {
        private Vector3 _targetPosition;
        private Vector3 _playerPosition;

        public HeroWalkToBedState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _targetPosition = _heroLogic.GetIOPositionByType(InteractiveObjectType.Bed);
            
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
                _heroLogic.ChangeState(_heroLogic.SleepState);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}