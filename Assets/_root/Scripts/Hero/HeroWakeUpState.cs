using Scripts.GlobalStateMachine;

namespace Scripts.Hero
{
    public class HeroWakeUpState : HeroBaseState
    {
        private readonly LocalEvents _localEvents;
        private readonly float _delayBeforeAnimation = 3.5f;
        private float _timer;
        private bool _animationPlayed;

        public HeroWakeUpState(HeroLogic heroLogic, LocalEvents localEvents) : base(heroLogic)
        {
            _localEvents = localEvents;
        }

        public override void Enter()
        {
            _timer = 0f;
            _animationPlayed = false;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _timer += deltaTime;

            if (!_animationPlayed && _timer >= _delayBeforeAnimation)
            {
                _animationPlayed = true;
                _heroLogic.PlayAnimation(HeroAnimationState.WakeUp, false);
                _heroLogic.SaveHeroState(_heroLogic.IdleState);
                _localEvents.TriggerHeroWokeUp();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}