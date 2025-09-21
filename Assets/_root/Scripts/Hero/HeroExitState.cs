using Scripts.GlobalStateMachine;
using Scripts.Rooms;

namespace Scripts.Hero
{
    public class HeroExitState : HeroBaseState
    {
        private LocalEvents _localEvents;

        public HeroExitState(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Chill, false);
            _heroLogic.SaveInitPos(InteractiveObjectType.Door);
            _heroLogic.SaveHeroState(_heroLogic.EnterState);
            _localEvents.TriggerHeroGetExit();
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}