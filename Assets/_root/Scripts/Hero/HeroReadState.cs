using Scripts.Progress;

namespace Scripts.Hero
{
    public class HeroReadState : HeroBaseState
    {
        private readonly ProgressDataAdapter _progressData;

        public HeroReadState(HeroLogic heroLogic, ProgressDataAdapter progressData) : base(heroLogic)
        {
            _progressData = progressData;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Read, true);
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