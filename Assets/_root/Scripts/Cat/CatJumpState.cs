using Scripts.Utils;
using UnityEngine;

namespace Scripts.Cat
{
    public class CatJumpState : CatBaseState
    {
        private CatTargetPosition _targetPosition;

        public CatJumpState(CatLogic catLogic) : base(catLogic)
        {
        }
        
        public override void Enter()
        {
            _catLogic.PlayTransitionAnimation(CatAnimationState.StartJumpUp, CatAnimationState.JumpUp, false, () =>
            {
                _catLogic.JumpToPosition(_targetPosition.JumpToTransform.position);
                _catLogic.SetSortingOrder(Consts.WindowSortingOrder);
            }, () =>
            {
                _catLogic.ChangeState(_catLogic.SeatState);
            });
            _targetPosition = _catLogic.GetCurrentTargetPosition();
            _catLogic.FlipSprite(_catLogic.GetCatPosition().x <= _targetPosition.JumpToTransform.position.x);
            _catLogic.SetCatOnFloor(false);
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