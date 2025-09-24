using Scripts.Utils;

namespace Scripts.Cat
{
    public class CatSeatState : CatBaseState
    {
        private float _timer;
        public CatSeatState(CatLogic catLogic) : base(catLogic)
        {
        }

        public override void Enter()
        {
            _timer = 0f;
            
            _catLogic.PlayTransitionAnimation(CatAnimationState.Seat, CatAnimationState.SeatTail, true);
            _catLogic.SetSortingOrder(Consts.WindowSortingOrder);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            _timer += deltaTime;
            if (_timer >= 3f)
            {
                _catLogic.ChangeState(_catLogic.SleepState);
            }
        }

        public override void Exit()
        {
           
        }
    }
}