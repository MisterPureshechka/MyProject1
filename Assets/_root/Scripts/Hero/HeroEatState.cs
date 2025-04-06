using Scripts.Progress;

namespace Scripts.Hero
{
    public class HeroEatState : HeroBaseState
    {
        private readonly ProgressDataAdapter _progressData;

        public HeroEatState(HeroLogic heroLogic, ProgressDataAdapter progressData) : base(heroLogic)
        {
            _progressData = progressData;
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