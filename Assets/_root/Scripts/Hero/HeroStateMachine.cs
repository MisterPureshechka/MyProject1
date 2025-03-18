using Scripts.GlobalStateMachine;

namespace Scripts.Hero
{
    public class HeroStateMachine 
    {
        public IState CurrentState { get; private set; }
        
        public void Init(IState startState)
        {
            CurrentState = startState;
            startState.Enter();
        }

        public void ChangeState(IState state) 
        {
            CurrentState.Exit();
            CurrentState = state;
            state.Enter();
        }  
    }
}