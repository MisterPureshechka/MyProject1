using _root.Notification;
using _root.RealEstate;
using _root.Scripts.Ui.Stats;
using Scripts.Animator;
using Scripts.Bugs;
using Scripts.Cat;
using Scripts.Catalogues;
using Scripts.Data;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.GameDev;
using Scripts.Hero;
using Scripts.Messenger;
using Scripts.Messenger.ComeBackLogic;
using Scripts.OnlineShop;
using Scripts.Passion;
using Scripts.Perks;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Sleep;
using Scripts.Sounds;
using Scripts.Stat;
using Scripts.Tasks;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using Scripts.Upgrade;
using Scripts.Wallet;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class GamePlanningState : BaseState
    {

        public GamePlanningState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData) : base(gameStateMachine, controllers, gameProgress, gameData)
        {
        }

        public override void Enter()
        {
            var progressStat = _gameProgress.LoadProgress();
            var progressDataAdapter = new ProgressDataAdapter(progressStat);
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