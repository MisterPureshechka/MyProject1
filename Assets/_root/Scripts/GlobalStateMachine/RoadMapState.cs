using _root.Planning;
using Scripts.Data;
using Scripts.Progress;
using Scripts.Ui;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class RoadMapState : BaseState
    {

        public RoadMapState(GameStateMachine gameStateMachine, Controllers controllers, SaveService saveService, GameData gameData, Canvas canvas) : base(gameStateMachine, controllers, saveService, gameData, canvas)
        {
        }

        public override void Enter()
        {
            var progressDataAdapter = new ProgressDataAdapter(_saveService);
            var localEvents = new LocalEvents();

            var uiFactory = new UiFactory(_gameData);
            var canvas = _canvas;
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