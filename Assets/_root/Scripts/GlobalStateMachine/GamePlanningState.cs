using Scripts.Data;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class GamePlanningState : BaseState
    {

        public GamePlanningState(GameStateMachine gameStateMachine, Controllers controllers, SaveService saveService, GameData gameData, Canvas canvas) : base(gameStateMachine, controllers, saveService, gameData, canvas)
        {
        }

        public override void Enter()
        {
            var progressStat = _saveService.LoadProgress();
            var progressDataAdapter = new ProgressDataAdapterOLD(progressStat);
            var localEvents = new LocalEvents();
            
            
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