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

        private void Start()
        {
            var gameProgress = new GameProgress();
            var controllers = new Controllers();
            _stateMachine = new GameStateMachine(controllers, _gameData, gameProgress, _loadingCurtain);
            
            _stateMachine.EnterState<LoadProgressState>();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _stateMachine.Update(deltaTime);
        }

        private void OnDestroy()
        {
        }
    }
}