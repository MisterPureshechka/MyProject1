namespace Scripts.Hero
{
    public class HeroBathState : HeroBaseState
    {
        public HeroBathState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Bath, false);
            _heroLogic.PlaceHero(_heroLogic.NormalizeVector(_heroLogic.GetTargetIO().RootObjectPosition.position));
            _heroLogic.FlipHero(false);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
        }

        public override void Exit()
        {
            base.Exit();
            _heroLogic.ResetHeroPosition();
        }
    }
}