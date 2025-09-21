using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroEnterState : HeroBaseState
    {
        private LocalEvents _localEvents;
        private float _timer;

        public HeroEnterState(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }
        
        public override void Enter()
        {
            _heroLogic.PlayAnimation(HeroAnimationState.Walk, false);
            Debug.Log("===========>>>>>>>>> Enter Hero");
            _timer = 0f;
        }

        public override void Update(float deltaTime)
        {  
            base.Update(deltaTime);
            _timer += deltaTime;

            if (_timer >= 2.5f)
            {
                _heroLogic.ChangeState(_heroLogic.IdleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}