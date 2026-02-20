using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;

namespace Scripts.Hero
{
    public class HeroChillState : HeroBaseState
    {
        private readonly ProgressDataAdapterOLD _progressData;
        private readonly LocalEvents _localEvents;

        public HeroChillState(HeroLogic heroLogic, ProgressDataAdapterOLD progressData, LocalEvents localEvents) : base(heroLogic)
        {
            _progressData = progressData;
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Chill, true);
            _heroLogic.FlipHero(false);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            //_heroLogic.TriggerActiveSprintByType(SprintType.Chill);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}