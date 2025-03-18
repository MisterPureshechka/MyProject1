using Scripts.Data;

namespace Scripts.Hero
{
    public class HeroIdleState : HeroBaseState
    {
        public HeroIdleState(HeroLogic heroLogic) : base(heroLogic)
        {
        }
        
        public override void Enter()
        {
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