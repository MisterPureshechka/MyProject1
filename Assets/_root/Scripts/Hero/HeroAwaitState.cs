using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroAwaitState : HeroBaseState
    {
        private IInteractiveObject _targetIO;
        private Vector3 _targetPosition;
        private Vector3 _playerPosition;

        public HeroAwaitState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            // _heroLogic.TriggerHeroGetIO();
            //
            // if (_targetIO.RootObjectPosition != null)
            // {
            //     _targetPosition = _heroLogic.NormalizeVector(_targetIO.RootObjectPosition.position);
            // }
            // else
            // {
            //     _targetPosition = _heroLogic.NormalizeVector(_targetIO.Position);
            // }
            //_heroLogic.TriggerHeroGetIO();
            
            _heroLogic.PlayAnimation(HeroAnimationState.Idle, true);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            
            //_playerPosition = _heroLogic.HeroPosition();
            
            // if (Vector3.Distance(_playerPosition, _targetPosition) < 0.25f)
            // {
            //     _heroLogic.PlaceHero(_heroLogic.NormalizeVector(_targetPosition));
            //     _heroLogic.ChangeStateByIOType(_targetIO.SprintType);
            // }
        }

        public override void Exit()
        {
        }
    }
}