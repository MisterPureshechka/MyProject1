using System;
using Scripts.Data;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public abstract class BaseState : IState, IDisposable
    {
        protected readonly GameStateMachine _gameStateMachine;
        protected readonly Controllers _controllers;
        protected readonly SaveService _saveService;
        protected readonly GameData _gameData;
        protected readonly Canvas _canvas;

        protected BaseState(GameStateMachine gsm, Controllers controllers, SaveService progress, GameData data, Canvas canvas)
        {
            _gameStateMachine = gsm;
            _controllers = controllers;
            _saveService = progress;
            _gameData = data;
            _canvas = canvas;
        }

        public abstract void Enter();
        public virtual void Update(float dt) { }
        public virtual void Exit() 
        { _controllers?.Dispose(); }
        public void Dispose() => Exit();
    }
}