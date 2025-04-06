using System.Collections.Generic;
using Scripts.Data;
using Scripts.Metadata;
using Scripts.Progress;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class LoadProgressState : BaseState
    {
        public LoadProgressState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData) : base(gameStateMachine, controllers, gameProgress, gameData)
        {
        }

        public override void Enter()
        {
            LoadProgressOrInitNew();

            _gameStateMachine.EnterState<MenuState>();
        }

        public override void Update(float deltaTime)
        {
        }

        public override void Exit()
        {
        }

        private void LoadProgressOrInitNew()
        {
            var progress = _gameProgress.LoadProgress();

            if (progress == null)
            {
                Debug.Log("Progress is null, creating new progress.");
                progress = NewProgress();
                _gameProgress.SaveProgress(progress);
            }
            
            Tools.SaveToJson(progress,Application.dataPath + "/metaconfig.json");
        }

        private ProgressData NewProgress()
        {
            var config = _gameData.MetadataConfig;
            var progress = new ProgressData(config.MetaFields);
            
            return progress;
        }
    }
}