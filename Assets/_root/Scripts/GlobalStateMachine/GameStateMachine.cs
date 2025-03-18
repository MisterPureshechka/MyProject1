using System;
using System.Collections.Generic;
using Scripts.Data;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class GameStateMachine 
    {
        private BaseState _currentBaseState;
        private readonly Controllers _controllers;
        private readonly GameData _gameData;
        private readonly GameProgress _gameProgress;
        private readonly LoadingCurtain _loadingCurtain;
        
        private readonly Dictionary<Type, BaseState> _cachedStates = new();

        public GameStateMachine(Controllers controllers, GameData gameData, GameProgress gameProgress,
            LoadingCurtain loadingCurtain)
        {
            _controllers = controllers;
            _gameData = gameData;
            _gameProgress = gameProgress;
            _loadingCurtain = loadingCurtain;
        }

        public void EnterState<T>() where T : BaseState
        {
            _currentBaseState?.Exit();
            _loadingCurtain?.Show();

            if (!_cachedStates.TryGetValue(typeof(T), out var state))
            {
                state = Activator.CreateInstance(typeof(T), this, _controllers, _gameProgress, _gameData) as BaseState;
                _cachedStates[typeof(T)] = state;
            }

            _currentBaseState = state;

            if (_currentBaseState != null)
                _currentBaseState.Enter();
            else
                Debug.LogError($"{typeof(T)} not found");

            _loadingCurtain?.Hide();
        }

        public void Update(float deltaTime)
        {
            _currentBaseState?.Update(deltaTime);
        }
    }
}