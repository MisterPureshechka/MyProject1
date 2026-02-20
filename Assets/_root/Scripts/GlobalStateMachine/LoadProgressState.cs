using Scripts.Data;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class LoadProgressState : BaseState
    {
        public LoadProgressState(GameStateMachine gameStateMachine, Controllers controllers, SaveService saveService, GameData gameData, Canvas canvas)
            : base(gameStateMachine, controllers, saveService, gameData, canvas) { }

        public override void Enter()
        {
            LoadProgressOrInitNew();
            _gameStateMachine.EnterState<ShopState>();
        }

        public override void Update(float deltaTime) { }

        public override void Exit() { }

        private void LoadProgressOrInitNew()
        {
            var progress = _saveService.LoadProgress();
            
            if (progress == null)
            {
                Debug.Log("Progress is null, creating new progress from meta_config.json.");
                progress = NewProgress();
                _saveService.SaveProgress(progress);
            }

            Tools.SaveToJson(progress, Application.dataPath + "/meta_config_backup.json");
        }

        private ProgressData NewProgress()
        {
            return new ProgressData();
        }
    }
}