using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroWalkToIOState : HeroBaseState
    {
        private IInteractiveObject _targetIO;
        private Vector3 _playerPosition;
        private Vector3 _targetPosition;

        public HeroWalkToIOState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _targetIO = _heroLogic.GetTargetIO();
            _targetPosition = _heroLogic.NormalizeVector(_targetIO.Position);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            
            _playerPosition = _heroLogic.HeroPosition();

            _heroLogic.MoveHero(_playerPosition, _targetPosition, deltaTime);

            if (Vector3.Distance(_playerPosition, _targetPosition) < 0.1f) 
            {
                _heroLogic.ChangeStateByIOType(_targetIO.ObjectType);
            }
        }

        public override void Exit()
        {
        }
    }
}