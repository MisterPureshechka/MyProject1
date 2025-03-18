namespace Scripts.Hero
{
    public class HeroDevState : HeroBaseState
    {
        public HeroDevState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
            _heroLogic.AnimateHero();
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            _heroLogic.ChangeProgressData();
        }

        public override void Exit()
        {
            _heroLogic.StopAnimation();
        }
    }
}