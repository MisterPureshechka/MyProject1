using Scripts.Progress;

namespace Scripts.Hero
{
    public class HeroSleepState : HeroBaseState
    {
        public HeroSleepState(HeroLogic heroLogic, ProgressDataAdapter progressData) : base(heroLogic)
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