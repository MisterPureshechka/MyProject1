using Scripts.Animator;
using Scripts.Data;
using Scripts.EcoSystem;
using Scripts.Hero;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Stat;
using Scripts.Tasks;
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
            var progressDataAdapter = new ProgressDataAdapter(progressStat);
            var localEvents = new LocalEvents();
            
            var uiFactory = new UiFactory(_gameData);
            
            var homeFactory = new HomeFactory(_gameData.PrefabDataBase); 
            var home = homeFactory.CreateRoom();
            var homeInitializer = new HomeInitializer(home);
            
            var heroFactory = new HeroFactory(_gameData.PrefabDataBase);
            var initialPos = homeInitializer.GetInitialPosition();
            var hero = heroFactory.CreateHero(initialPos);

            var interactiveObjectRegister = new InteractiveObjectRegisterer(home.InteractiveObjects);
            var camera = Camera.main;
            var canvas = Object.FindAnyObjectByType<Canvas>();

            var spriteAnimator = new SpriteAnimator();
            
            var inputController = new InputController(localEvents);

            var roomSize = homeInitializer.GetRoomSize();
            var heroMovementLogic =
                new HeroMovementLogic(camera, interactiveObjectRegister, inputController, localEvents);
            var heroLogic = new HeroLogic(_gameData.HeroConfig, heroMovementLogic, hero, initialPos, roomSize, spriteAnimator, progressDataAdapter, _gameProgress, localEvents);

            var interactiveObjectSelector = new InteractiveObjectSelector(camera, inputController, interactiveObjectRegister, localEvents);

            var iOGlobalAnimator =
                new InteractiveObjectGlobalAnimator(_gameData.InteractiveObjectConfig, interactiveObjectRegister);

            var bloomLogic = new WindowBloomLogic();

            var commandSystem = new CommandSystem(canvas, camera, uiFactory, localEvents);
            
            var hud = Object.FindAnyObjectByType<HUDView>();
            var statController = new StatsController(progressDataAdapter);
            var statEffectLogic = new StatEffectLogic(heroLogic, progressDataAdapter);

            var taskLibrary = new TaskLibrary();
            //var sprintSystem = new SprintSystem(taskLibrary, canvas, _gameData, hud.SprintView, uiFactory, localEvents);
            var sprintSystem = new SprintSystemTest(taskLibrary, canvas, _gameData, hud.SprintView, uiFactory, localEvents);

            var fader = new FaderLogic(localEvents);

            statController.RegisterView(hud.HealthBar);
            statController.RegisterView(hud.KnowledgeBar);
            statController.RegisterView(hud.PassionBar);
            statController.UpdateAllViews();

            var statsDebuger = Object.FindObjectOfType<StatsDebuger>();
            statsDebuger.Init(progressStat);

            _controllers.Add(inputController);
            _controllers.Add(heroLogic);
            _controllers.Add(interactiveObjectRegister);
            _controllers.Add(heroMovementLogic);
            _controllers.Add(interactiveObjectSelector);
            _controllers.Add(iOGlobalAnimator);
            _controllers.Add(statsDebuger); //temp
            _controllers.Add(spriteAnimator);
            _controllers.Add(bloomLogic);
            _controllers.Add(statController);
            _controllers.Add(statEffectLogic);
            _controllers.Add(sprintSystem);
            _controllers.Add(commandSystem);
            _controllers.Add(fader);
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