using Scripts.Progress;

namespace Scripts.Hero
{
    public class HeroChillState : HeroBaseState
    {
        private readonly ProgressDataAdapter _progressData;

        public HeroChillState(HeroLogic heroLogic, ProgressDataAdapter progressData) : base(heroLogic)
        {
            _progressData = progressData;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Chill, true);
            _heroLogic.FlipHero(false);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            _heroLogic.OnChillActiveStat?.Invoke();
        }

        public override void Exit()
        {
        }
    }
}