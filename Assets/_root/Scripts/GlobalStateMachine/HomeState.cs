using Scripts.Animator;
using Scripts.Data;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.Hero;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Stat;
using Scripts.Tasks;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
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
            var cameraLogic = new CameraLogic(camera, localEvents, _gameData.InteractiveObjectConfig);
            
            var canvas = Object.FindAnyObjectByType<Canvas>();
            var timeView = Object.FindAnyObjectByType<TimeView>();
            var timeLogic = new TimeLogic(progressDataAdapter, timeView, localEvents);
            
            var spriteAnimator = new SpriteAnimator();
            
            var inputController = new InputController(localEvents);

            var roomSize = homeInitializer.GetRoomSize();
            var heroMovementLogic =
                new HeroMovementLogic(camera, interactiveObjectRegister, inputController, localEvents);
            var heroLogic = new HeroLogic(_gameData.HeroConfig, heroMovementLogic, hero, initialPos, roomSize, spriteAnimator, progressDataAdapter, _gameProgress, localEvents);

            var interactiveObjectSelector = new InteractiveObjectSelector(camera, inputController, interactiveObjectRegister, localEvents);

            var iOGlobalAnimator =
                new InteractiveObjectGlobalAnimator(_gameData.InteractiveObjectConfig, interactiveObjectRegister);

            var bloomLogic = new WindowBloomLogic(localEvents);
            var skyLogic = new SkyLogic(localEvents, Object.FindAnyObjectByType<SkyView>());
            var volumeLogic = new VolumeLogic(localEvents, _gameData.InteractiveObjectConfig);
            var calendarLogic = new CalendarLogic(localEvents, Object.FindAnyObjectByType<MiniCalendarView>(), progressDataAdapter);

            var commandSystem = new CommandSystem(canvas, camera, uiFactory, localEvents);
            
            var hud = Object.FindAnyObjectByType<HUDView>();
            var statController = new StatsController(progressDataAdapter, localEvents);
            var statEffectLogic = new StatEffectLogic(progressDataAdapter, localEvents);

            var sideRoomChecker = new SideRoomChecker(home, localEvents);

            var taskLibrary = new TaskLibrary(progressDataAdapter, localEvents);
            //var sprintSystem = new SprintSystem(taskLibrary, canvas, _gameData, hud.SprintView, uiFactory, localEvents);
            var sprintSystem = new SprintSystem(taskLibrary, canvas, _gameData, hud.SprintView, uiFactory, localEvents, interactiveObjectRegister, progressDataAdapter);

            var fader = new FaderLogic(localEvents);
            
            var tooltipLogic = new TooltipStatLogic(progressDataAdapter, uiFactory.GetTooltip(canvas.transform), _gameData.PrefabDataBase, localEvents, canvas);

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
            _controllers.Add(sideRoomChecker);
            _controllers.Add(cameraLogic);
            _controllers.Add(tooltipLogic);
            _controllers.Add(timeLogic);
            _controllers.Add(skyLogic);
            _controllers.Add(volumeLogic);
            _controllers.Add(calendarLogic);
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