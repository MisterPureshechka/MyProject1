using Scripts.Progress;

namespace Scripts.Hero
{
    public class HeroEatState : HeroBaseState
    {
        private readonly ProgressDataAdapterOLD _progressData;

        public HeroEatState(HeroLogic heroLogic, ProgressDataAdapterOLD progressData) : base(heroLogic)
        {
            _progressData = progressData;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Eat, true);
            _heroLogic.FlipHero(false);
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