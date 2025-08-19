using System;
using Scripts.Data;
using Scripts.Progress;

namespace Scripts.GlobalStateMachine
{
    public abstract class BaseState : IState, IDisposable
    {
        protected readonly GameStateMachine _gameStateMachine;
        protected readonly Controllers _controllers;
        protected readonly GameProgress _gameProgress;
        protected readonly GameData _gameData;

        protected BaseState(GameStateMachine gsm, Controllers controllers, GameProgress progress, GameData data)
        {
            _gameStateMachine = gsm;
            _controllers = controllers;
            _gameProgress = progress;
            _gameData = data;
        }

        public abstract void Enter();
        public virtual void Update(float dt) { }
        public virtual void Exit() 
        { _controllers?.Dispose(); }
        public void Dispose() => Exit();
    }
}