using Scripts.Data;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroWalkState : HeroBaseState
    {
        private Vector3 _playerPosition;
        private Vector3 _targetPosition;
        private IInteractiveObject _targetIO;
        public HeroWalkState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _targetPosition = _heroLogic.GetTargetPosition();
            _heroLogic.PlayAnimation(HeroAnimationState.Walk, true);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            
            _playerPosition = _heroLogic.HeroPosition();

            _heroLogic.MoveHero(_playerPosition, _targetPosition, deltaTime);

            if (Vector3.Distance(_playerPosition, _targetPosition) < 0.1f) 
            {
                _heroLogic.ChangeState(_heroLogic.IdleState);
            }
        }

        public override void Exit()
        {
        }
    }
}