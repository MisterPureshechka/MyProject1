using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;

namespace Scripts.Hero
{
    public class HeroDevState : HeroBaseState
    {
        private ProgressDataAdapterOLD _progressData;
        private readonly LocalEvents _localEvents;

        public HeroDevState(HeroLogic heroLogic, ProgressDataAdapterOLD progressDataAdapterOld, LocalEvents localEvents) : base(heroLogic)
        {
            _progressData = progressDataAdapterOld;
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Dev, true);
            //_heroLogic.PlayTransitionAnimation(HeroAnimationState.Read, HeroAnimationState.Dev);
            _heroLogic.FlipHero(false);
            _heroLogic.ChangeSortingOrder(-4);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
        }

        public override void Exit()
        {
            base.Exit();
            _heroLogic.TiggerSprintExit();
            _heroLogic.ChangeSortingOrder(4);
        }
    }
}