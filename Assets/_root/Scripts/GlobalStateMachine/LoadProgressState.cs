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
        }

        private ProgressData NewProgress()
        {
            var config = _gameData.MetadataConfig;
            var progress = new ProgressData();
            Debug.Log($"Before init \n Food = {progress.Metadata[Consts.Food].Value}\n Energy = {progress.Metadata[Consts.Energy].Value}");
            progress.Metadata[Consts.Energy].Init(config.StartEnergy);
            progress.Metadata[Consts.Food].Init(config.StartFood);

            Debug.Log($"New progress \n Food = {config.StartFood}\n Energy = {config.StartEnergy}");
            Debug.Log($"After init \n Food = {progress.Metadata[Consts.Food]}\n Energy = {progress.Metadata[Consts.Energy]}");
            return progress;
        }
    }
}