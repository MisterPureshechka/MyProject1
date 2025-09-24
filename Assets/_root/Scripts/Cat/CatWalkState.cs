using Scripts.Utils;
using UnityEngine;

namespace Scripts.Cat
{
    public class CatWalkState : CatBaseState
    {
        private CatTargetPosition _targetPosition;

        public CatWalkState(CatLogic catLogic) : base(catLogic)
        {
        }
        
        public override void Enter()
        {
            if (!_catLogic.IsOnFloor)
            {
                _catLogic.ChangeState(_catLogic.JumpDownState);
                return;
            }
            
            _catLogic.PlayAnimation(CatAnimationState.Walk, true);
            
            _targetPosition = _catLogic.GetNewTargetPosition();
            _catLogic.FlipSprite(_catLogic.GetCatPosition().x <= _targetPosition.Transform.position.x);
            _catLogic.SetSortingOrder(Consts.CatSortingOrder);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            var targetPos = _catLogic.NormalizeVector(_targetPosition.Transform.position);
            
            _catLogic.MoveCatToPosition(targetPos, deltaTime);
            
            if (Vector3.Distance(_catLogic.GetCatPosition(), targetPos) < 0.1f) 
            {
                if (_targetPosition.JumpToTransform)
                {
                    _catLogic.ChangeState(_catLogic.JumpState);
                }
                else
                {
                    _catLogic.ChangeState(_catLogic.SeatState);
                }
            }
        }

        public override void Exit()
        {
           
        }
    }
}