using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Data;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class GameStateMachine
    {
        private BaseState _currentBaseState;
        private readonly GameData _gameData;
        private readonly SaveService _saveService;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ICoroutineRunner _runner;
        private readonly Canvas _canvas;

        private readonly Dictionary<Type, BaseState> _cachedStates = new();

        public GameStateMachine(ICoroutineRunner runner, GameData gameData, SaveService saveService, LoadingCurtain loadingCurtain, Canvas canvas)
        {
            _runner = runner;
            _gameData = gameData;
            _saveService = saveService;
            _loadingCurtain = loadingCurtain;
            _canvas = canvas;
        }

        public void EnterState<T>() where T : BaseState
        {
            _runner.StartCoroutine(EnterStateRoutine<T>());
        }

        private IEnumerator EnterStateRoutine<T>() where T : BaseState
        {
            if (_loadingCurtain != null)
                yield return _loadingCurtain.ShowRoutine();

            _currentBaseState?.Exit();

            if (!_cachedStates.TryGetValue(typeof(T), out var state))
            {
                var controllers = new Controllers();
                state = Activator.CreateInstance(typeof(T), this, controllers, _saveService, _gameData, _canvas) as BaseState;
                _cachedStates[typeof(T)] = state;
            }

            _currentBaseState = state ?? throw new Exception($"{typeof(T)} not found");
            _currentBaseState.Enter();
            yield return new WaitForSeconds(1f);
            
            if (_loadingCurtain != null)
                yield return _loadingCurtain.HideRoutine();
        }

        public void Update(float deltaTime)
        {
            _currentBaseState?.Update(deltaTime);
        }
    }
}
