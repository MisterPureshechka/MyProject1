namespace Scripts.Hero
{
    public class HeroCleanState : HeroBaseState
    {
        public HeroCleanState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Toilet, true);
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