using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Rooms;

namespace Scripts.Hero
{
    public class HeroSleepState : HeroBaseState
    {
        private LocalEvents _localEvents;
        public HeroSleepState(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.GoToBed, false);
            _heroLogic.SaveHeroState(_heroLogic.WakeUpState);
            _heroLogic.SaveInitPos(InteractiveObjectType.Bed);
            _localEvents.TriggerSleepState();
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