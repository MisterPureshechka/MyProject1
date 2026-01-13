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
using Scripts.Job;
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
            
            var heroAnimator = new HeroAnimator(_gameData.HeroConfig, hero, localEvents);
            var eyesMoodStateLogic = new EyesMoodLogic(progressDataAdapter, localEvents);
            var bodyMoodStateLogic = new BodyStateLogic(progressDataAdapter, localEvents);
            var cleanStateLogic = new ShowerStateLogic(progressDataAdapter, localEvents, hero);
            var coffeeLogic = new CoffeeLogic(localEvents);

            var interactiveObjectRegister = new InteractiveObjectRegisterer(home.InteractiveObjects);
            var camera = Camera.main;
            var cameraLogic = new CameraLogicOLD(camera, localEvents, _gameData.InteractiveObjectConfig);
            
            var canvas = Object.FindAnyObjectByType<Canvas>();
            
            
            var speechBubble = new SpeechBubbleLogic(Object.FindAnyObjectByType<SpeechBubbleView>(FindObjectsInactive.Include), localEvents, hero, camera);
            
            var spriteAnimator = new SpriteAnimator();
            
            var inputController = new InputController(canvas, localEvents);

            var roomSize = homeInitializer.GetRoomSize();
            var heroMovementLogic =
                new HeroMovementLogic(camera, interactiveObjectRegister, inputController, localEvents);
            var heroLogic = new HeroLogic(_gameData.HeroConfig, heroAnimator, heroMovementLogic, hero, initialPos, roomSize, spriteAnimator, progressDataAdapter, _gameProgress, localEvents, interactiveObjectRegister);

            var interactiveObjectSelector = new InteractiveObjectSelector(camera, inputController, interactiveObjectRegister, localEvents);

            var iOGlobalAnimator =
                new InteractiveObjectGlobalAnimator(_gameData.InteractiveObjectConfig, interactiveObjectRegister);

            var bloomLogic = new WindowBloomLogic(localEvents);
            var skyView = homeFactory.CreateSky();
            var loopingMover = new LoopingMoverGroup(skyView.StarsPrefabs, 3f, 5f);
            var skyLogic = new SkyLogic(loopingMover, localEvents, skyView);
            var volumeLogic = new VolumeLogic(localEvents, _gameData.InteractiveObjectConfig);
            var calendarLogic = new CalendarLogic(localEvents, Object.FindAnyObjectByType<MiniCalendarView>(FindObjectsInactive.Include), Object.FindAnyObjectByType<CalendarCatalogue>(FindObjectsInactive.Include), progressDataAdapter);

            var commandSystem = new CommandSystem(canvas, camera, uiFactory, localEvents);

            var perkService = new PerkService(uiFactory, canvas, localEvents, progressDataAdapter, _gameProgress);
            
            var hud = Object.FindAnyObjectByType<HUDView>(FindObjectsInactive.Include);
            hud.gameObject.SetActive(true);
            var statController = new StatsController(progressDataAdapter, localEvents);
            var statEffectLogic = new StatEffectLogic(progressDataAdapter, localEvents, perkService);
            
            var timeView = Object.FindAnyObjectByType<TimeView>(FindObjectsInactive.Include);
            var timeLogic = new TimeLogic(progressDataAdapter, timeView, localEvents);
            
            var sideRoomChecker = new SideRoomChecker(home, localEvents);
            var healthStatLogic = new HealthStatLogic(Object.FindAnyObjectByType<HealthStatPanel>(FindObjectsInactive.Include), progressDataAdapter, localEvents);
            var skillStatLogic =
                new SkillStatLogic(Object.FindAnyObjectByType<SkillStatPanel>(FindObjectsInactive.Include),
                    progressDataAdapter, localEvents);

            var bugLogic = new BugLogic(progressDataAdapter);
            var gameDevProgress = new GameDevProgress(progressDataAdapter); //надо диспозить
            var taskLibrary = new TaskLibrary(progressDataAdapter, bugLogic, localEvents);
            var sprintSystem = new SprintSystemOLD(taskLibrary, canvas, _gameData, hud.SprintView, uiFactory, localEvents, interactiveObjectRegister, progressDataAdapter, perkService, gameDevProgress);
            var gameDevProgressPanelLogic = new GameDevProgressPanelLogic(Object.FindAnyObjectByType<GameDevProgressPanel>(FindObjectsInactive.Include), gameDevProgress, taskLibrary, localEvents);

            var fader = new FaderLogic(localEvents);

            var wallet = new WalletLogic(progressDataAdapter, _gameProgress, localEvents);
            var onlineShopController = new OnlineShopController(Object.FindAnyObjectByType<OnlineShopView>(FindObjectsInactive.Include),
                localEvents, _gameData.PrefabDataBase, new ShopItemsLibrary(), wallet);
            var upgradeLogic = new UpgradeLogicOLD(localEvents, progressDataAdapter);
            
            var tooltipLogic = new TooltipStatLogic(progressDataAdapter, uiFactory.GetTooltip(canvas.transform), _gameData.PrefabDataBase, localEvents, canvas);
            var catalogueManager = new CatalogueManager(localEvents);

            var notificationSystem = new NotificationSystem(new NotificationLibrary(), calendarLogic, localEvents, timeLogic);
            var roomShaker = new RoomShaker(home, localEvents, _gameData.InteractiveObjectConfig);

            var jobLogic = new JobLogic(progressDataAdapter, new JobLibrary(), localEvents, calendarLogic, timeLogic);
            var comeBackStore = new ComeBackStore();
            var jobMessageGenerator = new JobMessageGenerator(calendarLogic, localEvents, _gameData.MessengerConfig, timeLogic, comeBackStore, jobLogic);
            var messenger = new MessengerLogic(localEvents, _gameData.MessengerConfig, calendarLogic, timeLogic);
            var clickLogic = new ClickLogic.ClickLogic(localEvents);
            var roomColliderController = new RoomColliderController(home, localEvents);
            
            var upgradeAndCleanLogic = new UpgradeLogic(
                progressDataAdapter, 
                localEvents, 
                _gameData.UpgradableConfig, 
                interactiveObjectRegister.GetIOByType(InteractiveObjectType.Pc).SpriteRenderer, 
                interactiveObjectRegister.GetIOByType(InteractiveObjectType.Chair).SpriteRenderer
                );

            var rentLogic = new RentLogic(localEvents, calendarLogic, progressDataAdapter);

            var roomExitLogic = new RoomExitLogic(_gameStateMachine, localEvents, home, progressDataAdapter, _gameProgress);
            var sleepLogic = new SleepLogic(_gameStateMachine, localEvents, progressDataAdapter, _gameProgress, timeLogic);

            var catFactory = new CatFactory(_gameData.PrefabDataBase);
            var catPositionRegisterer = new CatPositionRegisterer();
            var catLogic = new CatLogic(localEvents, initialPos, catFactory.CreateCat(), catPositionRegisterer, _gameData.CatConfig);
            
            var soundService = new SoundService(localEvents, _gameData.SoundConfig);

            statController.RegisterView(hud.HealthBar);
            statController.RegisterView(hud.KnowledgeBar);
            statController.UpdateAllViews();

            var passionLogic = new PassionLogic(localEvents, progressDataAdapter, _gameProgress, hud.PassionBar);
            

            _controllers.Add(inputController);
            _controllers.Add(heroAnimator);
            _controllers.Add(heroLogic);
            _controllers.Add(interactiveObjectRegister);
            _controllers.Add(heroMovementLogic);
            _controllers.Add(interactiveObjectSelector);
            _controllers.Add(iOGlobalAnimator);
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
            _controllers.Add(sleepLogic);
            _controllers.Add(catLogic);
            _controllers.Add(passionLogic);
            _controllers.Add(perkService);
            _controllers.Add(healthStatLogic);
            _controllers.Add(skillStatLogic);
            _controllers.Add(gameDevProgressPanelLogic);
            _controllers.Add(eyesMoodStateLogic);
            _controllers.Add(bodyMoodStateLogic);
            _controllers.Add(cleanStateLogic);
            _controllers.Add(coffeeLogic);
            _controllers.Add(speechBubble);
            _controllers.Add(upgradeAndCleanLogic);
            _controllers.Add(soundService);
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