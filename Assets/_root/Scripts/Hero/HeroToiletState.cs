namespace Scripts.Hero
{
    public class HeroToiletState : HeroBaseState
    {
        public HeroToiletState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Toilet, true);
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