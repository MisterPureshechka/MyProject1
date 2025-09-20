namespace Scripts.Hero
{
    public class HeroWakeUpState : HeroBaseState
    {
        public HeroWakeUpState(HeroLogic heroLogic) : base(heroLogic)
        {
        }

        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Chill, true);
            _heroLogic.SaveHeroState(_heroLogic.IdleState);
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