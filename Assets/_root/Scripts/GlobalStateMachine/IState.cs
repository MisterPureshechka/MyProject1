namespace Scripts.GlobalStateMachine
{
    public interface IState
    {
        void Enter();
        
        void Update(float deltaTime);
        
        void Exit();
    }
}