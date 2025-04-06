using Scripts.Progress;
using Scripts.Utils;

namespace Scripts.Hero
{
    public class HeroDevState : HeroBaseState
    {
        private ProgressDataAdapter _progressData;
        public HeroDevState(HeroLogic heroLogic, ProgressDataAdapter progressDataAdapter) : base(heroLogic)
        {
            _progressData = progressDataAdapter;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Dev, true);
            //_heroLogic.PlayTransitionAnimation(HeroAnimationState.Read, HeroAnimationState.Dev);
            _heroLogic.FlipHero(false);
            _heroLogic.ChangeSortingOrder(Consts.HeroSortingOrder - 2);
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            _heroLogic.OnDevActiveStat?.Invoke();
        }

        public override void Exit()
        {
            _heroLogic.SaveProgress();
            _heroLogic.ChangeSortingOrder(Consts.HeroSortingOrder);
        }
    }
}