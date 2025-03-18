using Scripts.Data;
using Scripts.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.GlobalStateMachine
{
    public class MenuState : BaseState
    {
        private GameObject _menu;
        private Button _newGameButton;


        public MenuState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData) : base(gameStateMachine, controllers, gameProgress, gameData)
        {
            
        }

        public override void Enter()
        {
            InitUi();
        }

        private void InitUi()
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            
            _menu = Object.Instantiate(_gameData.PrefabDataBase.Menu, canvas.transform);
            _menu.transform.SetAsFirstSibling();
            
            _newGameButton = _menu.GetComponentInChildren<Button>();
            _newGameButton.onClick.AddListener(ChangeState);
        }

        public override void Update(float deltaTime)
        {
        }

        private void ChangeState()
        {
            _gameStateMachine.EnterState<HomeState>();
        }

        public override void Exit()
        {
            Object.Destroy(_menu);
            _newGameButton.onClick.RemoveAllListeners();
        }
    }
}