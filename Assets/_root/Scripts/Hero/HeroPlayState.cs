using Scripts.Progress;

namespace Scripts.Hero
{
    public class HeroPlayState : HeroBaseState
    {
        private readonly ProgressDataAdapter _progressData;

        public HeroPlayState(HeroLogic heroLogic, ProgressDataAdapter progressData) : base(heroLogic)
        {
            _progressData = progressData;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Play, true);
            _heroLogic.FlipHero(false);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
        }

        public override void Exit()
        {
        }
    }
}