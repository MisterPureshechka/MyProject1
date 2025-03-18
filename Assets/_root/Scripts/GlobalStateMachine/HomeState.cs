using Scripts.Data;
using Scripts.Hero;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Ui;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class HomeState : BaseState
    {

        public HomeState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData) : base(gameStateMachine, controllers, gameProgress, gameData)
        {
        }

        public override void Enter()
        {
            var progressStat = _gameProgress.LoadProgress();
            var homeFactory = new HomeFactory(_gameData.PrefabDataBase);
            var home = homeFactory.CreateRoom();
            var homeInitializer = new HomeInitializer(home);
            
            var heroFactory = new HeroFactory(_gameData.PrefabDataBase);
            var initialPos = homeInitializer.GetInitialPosition();
            var hero = heroFactory.CreateHero(initialPos);

            var interactiveObjectRegister = new InteractiveObjectRegisterer(home.InteractiveObjects);
            var camera = Camera.main;
            
            var inputController = new InputController();

            var roomSize = homeInitializer.GetRoomSize();
            var heroMovementLogic =
                new HeroMovementLogic(camera, interactiveObjectRegister, inputController);
            var heroLogic = new HeroLogic(_gameData.HeroConfig, heroMovementLogic, hero, initialPos, roomSize, progressStat);
            
            var interactiveObjectSelector = new InteractiveObjectSelector(camera, inputController, interactiveObjectRegister);

            var iOGlobalAnimator =
                new InteractiveObjectGlobalAnimator(_gameData.InteractiveObjectConfig, interactiveObjectRegister);

            
            var statsDebuger = Object.FindObjectOfType<StatsDebuger>();
            statsDebuger.Init(progressStat);

            _controllers.Add(inputController);
            _controllers.Add(heroLogic);
            _controllers.Add(interactiveObjectRegister);
            _controllers.Add(heroMovementLogic);
            _controllers.Add(interactiveObjectSelector);
            _controllers.Add(iOGlobalAnimator);
            _controllers.Add(statsDebuger); //temp
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