using Scripts.Data;
using Scripts.Progress;

namespace Scripts.GlobalStateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly GameStateMachine _gameStateMachine;
        protected readonly Controllers _controllers;
        protected readonly GameProgress _gameProgress;
        protected readonly GameData _gameData;

        protected BaseState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData)
        {
            _gameStateMachine = gameStateMachine;
            _controllers = controllers;
            _gameProgress = gameProgress;
            _gameData = gameData;
        }

        public abstract void Enter();
        public abstract void Exit();
        
        public abstract void Update(float deltaTime);
    }
}