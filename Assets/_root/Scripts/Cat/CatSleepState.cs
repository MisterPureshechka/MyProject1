namespace Scripts.Cat
{
    public class CatSleepState : CatBaseState
    {
        private float _timer;
        public CatSleepState(CatLogic catLogic) : base(catLogic)
        {
        }
        
        public override void Enter()
        {
            _timer = 0f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            _timer += deltaTime;

            if (_timer >= 5f)
            {
                _catLogic.ChangeState(_catLogic.WalkState);
            }
        }

        public override void Exit()
        {
           
        }
    }
}