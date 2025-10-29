using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroWalkToSprint : HeroBaseState
    {
        private readonly LocalEvents _localEvents;
        private IInteractiveObject _targetIO;
        private Vector3 _playerPosition;
        private Vector3 _targetPosition;
        
        private SprintType _desiredSprintType = SprintType.None;

        public HeroWalkToSprint(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.SetWalking(true);
            _targetIO = _heroLogic.GetTargetIO();
            if (_targetIO == null)
            {
                _heroLogic.ChangeState(_heroLogic.IdleState);
                return;
            }
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
                _localEvents.TriggerHeroGetSprint(_desiredSprintType != SprintType.None 
                    ? _desiredSprintType 
                    : _targetIO.SprintType);
            }
        }

        public override void Exit()
        {
            _heroLogic.SetWalking(false);
            base.Exit();
        }
        
        public void SetDesiredSprintType(SprintType sprintType) => _desiredSprintType = sprintType;
    }
}