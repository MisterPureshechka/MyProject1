using _root.Notification;
using _root.RealEstate;
using Scripts.Animator;
using Scripts.Catalogues;
using Scripts.Data;
using Scripts.EcoSystem;
using Scripts.EcoSystem.Calendar;
using Scripts.Hero;
using Scripts.Job;
using Scripts.Messenger;
using Scripts.Messenger.ComeBackLogic;
using Scripts.OnlineShop;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Stat;
using Scripts.Tasks;
using Scripts.Ui;
using Scripts.Ui.TaskUi;
using Scripts.Upgrade;
using Scripts.Wallet;
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
            var sky = Object.FindAnyObjectByType<SkyView>(FindObjectsInactive.Include);
            var loopingMover = new LoopingMoverGroup(sky.StarsPrefabs, 3f, 5f);
            var skyLogic = new SkyLogic(loopingMover, localEvents, Object.FindAnyObjectByType<SkyView>());
            var volumeLogic = new VolumeLogic(localEvents, _gameData.InteractiveObjectConfig);
            var calendarLogic = new CalendarLogic(localEvents, Object.FindAnyObjectByType<MiniCalendarView>(FindObjectsInactive.Include), Object.FindAnyObjectByType<CalendarCatalogue>(FindObjectsInactive.Include), progressDataAdapter);

            var commandSystem = new CommandSystem(canvas, camera, uiFactory, localEvents);
            
            var hud = Object.FindAnyObjectByType<HUDView>(FindObjectsInactive.Include);
            var statController = new StatsController(progressDataAdapter, localEvents);
            var statEffectLogic = new StatEffectLogic(progressDataAdapter, localEvents);

            var sideRoomChecker = new SideRoomChecker(home, localEvents);

            var taskLibrary = new TaskLibrary(progressDataAdapter, localEvents);
            var sprintSystem = new SprintSystem(taskLibrary, canvas, _gameData, hud.SprintView, uiFactory, localEvents, interactiveObjectRegister, progressDataAdapter);

            var fader = new FaderLogic(localEvents);

            var wallet = new WalletLogic(progressDataAdapter, _gameProgress, localEvents);
            var onlineShopController = new OnlineShopController(Object.FindAnyObjectByType<OnlineShopView>(FindObjectsInactive.Include),
                localEvents, _gameData.PrefabDataBase, new ShopItemsLibrary(), wallet);
            var upgradeLogic = new UpgradeLogic(localEvents, progressDataAdapter);
            
            var tooltipLogic = new TooltipStatLogic(progressDataAdapter, uiFactory.GetTooltip(canvas.transform), _gameData.PrefabDataBase, localEvents, canvas);
            var catalogueManager = new CatalogueManager(localEvents);

            var notificationSystem = new NotificationSystem(new NotificationLibrary(), calendarLogic, localEvents, timeLogic);
            var roomShaker = new RoomShaker(home, localEvents, _gameData.InteractiveObjectConfig);

            var jobLogic = new JobLogic(progressDataAdapter, new JobLibrary(), localEvents, calendarLogic);
            var comeBackStore = new ComeBackStore();
            var jobMessageGenerator = new JobMessageGenerator(calendarLogic, localEvents, _gameData.MessengerConfig, timeLogic, comeBackStore, jobLogic);
            var messenger = new MessengerLogic(localEvents, _gameData.MessengerConfig, calendarLogic, timeLogic);
            var clickLogic = new ClickLogic.ClickLogic(localEvents);
            var roomColliderController = new RoomColliderController(home, localEvents);

            var rentLogic = new RentLogic(localEvents, calendarLogic, progressDataAdapter);

            var roomExitLogic = new RoomExitLogic(_gameStateMachine, localEvents, home);

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
            _controllers.Add(loopingMover);
            _controllers.Add(skyLogic);
            _controllers.Add(volumeLogic);
            _controllers.Add(calendarLogic);
            _controllers.Add(catalogueManager);
            _controllers.Add(onlineShopController);
            _controllers.Add(wallet);
            _controllers.Add(upgradeLogic);
            _controllers.Add(notificationSystem);
            _controllers.Add(roomShaker);
            _controllers.Add(jobLogic);
            _controllers.Add(roomExitLogic);
            _controllers.Add(jobMessageGenerator);
            _controllers.Add(messenger);
            _controllers.Add(clickLogic);
            _controllers.Add(roomColliderController);
            _controllers.Add(rentLogic);
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