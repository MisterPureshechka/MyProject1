using _root.Planning;
using Scripts.Data;
using Scripts.Progress;
using Scripts.Ui;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class RoadMapState : BaseState
    {

        public RoadMapState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData) : base(gameStateMachine, controllers, gameProgress, gameData)
        {
        }

        public override void Enter()
        {
            var progressStat = _gameProgress.LoadProgress();
            var progressDataAdapter = new ProgressDataAdapter(progressStat);
            var localEvents = new LocalEvents();

            var uiFactory = new UiFactory(_gameData);
            var canvas = Object.FindAnyObjectByType<Canvas>();
            var roadMapView = uiFactory.GetRoadMapView(canvas.transform);
            roadMapView.transform.SetAsFirstSibling();
            var levelNodePrefab = uiFactory.GetLevelNodePrefab();
            var connectorPrefab = uiFactory.GetConnectorPrefab();
            var levelMapController = new LevelMapController(_gameStateMachine, progressDataAdapter, _gameData.LevelMapConfig, roadMapView, levelNodePrefab, connectorPrefab);

            _controllers.Add(levelMapController);
        }

        public override void Update(float deltaTime)
        {
            _controllers.Execute(deltaTime);
        }

        public override void Exit()
        {
            _controllers.CleanUp();
        }
    }

}