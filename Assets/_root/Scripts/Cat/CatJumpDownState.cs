using Scripts.Utils;
using UnityEngine;

namespace Scripts.Cat
{
    public class CatJumpDownState : CatBaseState
    {
        private CatTargetPosition _targetPosition;

        public CatJumpDownState(CatLogic catLogic) : base(catLogic)
        {
        }
        
        public override void Enter()
        {
            _catLogic.PlayTransitionAnimation(CatAnimationState.StartJumpDown, CatAnimationState.JumpDown, false, () =>
            {
                _catLogic.SetSortingOrder(Consts.CatSortingOrder);
                _catLogic.JumpDownToPosition(_targetPosition.Transform.position);
            }, () =>
            {
                _catLogic.ChangeState(_catLogic.WalkState);
            });
            _targetPosition = _catLogic.GetCurrentTargetPosition();
            _catLogic.FlipSprite(_catLogic.GetCatPosition().x <= _targetPosition.Transform.position.x);
            _catLogic.SetCatOnFloor(true);
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