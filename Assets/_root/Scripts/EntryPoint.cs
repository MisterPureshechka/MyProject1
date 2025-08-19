using System.Collections;
using UnityEngine;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;

namespace Scripts
{
    public class EntryPoint : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private GameData _gameData;
        [SerializeField] private LoadingCurtain _loadingCurtain;

        private GameStateMachine _stateMachine;

        public Coroutine StartCoroutine(IEnumerator routine) => base.StartCoroutine(routine);

        private void Start()
        {
            var gameProgress = new GameProgress();
            _stateMachine = new GameStateMachine(this, _gameData, gameProgress, _loadingCurtain);

            _stateMachine.EnterState<LoadProgressState>(); 
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }
    }
}