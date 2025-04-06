using UnityEngine;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Progress;

namespace Scripts
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameData _gameData;
        [SerializeField] private LoadingCurtain _loadingCurtain;

        private GameStateMachine _stateMachine;
        private Controllers _controllers;

        private void Start()
        {
            var gameProgress = new GameProgress();
            _controllers = new Controllers();
            _stateMachine = new GameStateMachine(_controllers, _gameData, gameProgress, _loadingCurtain);
            
            _stateMachine.EnterState<LoadProgressState>();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _stateMachine.Update(deltaTime);
        }

        private void OnDestroy()
        {
            _controllers.CleanUp();
        }
    }
}