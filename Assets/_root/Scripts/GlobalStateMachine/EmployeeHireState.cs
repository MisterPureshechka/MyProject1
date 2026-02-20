using _root.Scripts.Rooms.SlotLogic.SlotSelectorFeatures;
using _root.Scripts.Ui.Stats;
using Scripts.Data;
using Scripts.EmployeeLogic;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Rooms.RoomItems;
using Scripts.Rooms.Scripts.Rooms;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using Scripts.Ui;
using Scripts.Ui.EmployeeShop;
using Scripts.Ui.ItemShop;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class EmployeeHireState : BaseState
    {
        public EmployeeHireState(GameStateMachine gameStateMachine, Controllers controllers, SaveService saveService, GameData gameData, Canvas canvas) : base(gameStateMachine, controllers, saveService, gameData, canvas)
        {
        }

        public override void Enter()
        {
            var progress = _saveService.LoadProgress();
            var progressDataAdapter = new ProgressDataAdapter(_saveService);
            var localEvents = new LocalEvents();
            var camera = Camera.main;
            var homeFactory = new HomeFactory(_gameData.PrefabDataBase);
            var uiFactory = new UiFactory(_gameData);
            var canvas = Object.FindAnyObjectByType<Canvas>();

            var room = new Room(progressDataAdapter.Data.OfficeCells);
            var roomView = homeFactory.CreateRoomView();
            var roomLogic = new RoomLogic(room, _saveService, progressDataAdapter);
            var roomItemViewFactory = new RoomItemViewFactory();
            var employeeViewFactory = new EmployeeViewFactory(_gameData.PrefabDataBase.employeeItemPrefab);
            var roomVisuals = new RoomVisuals(roomView, roomLogic, roomItemViewFactory, employeeViewFactory, localEvents);
            var cameraLogic = new CameraLogic(camera, roomVisuals.GetAverageSlotPosition());
            var roomFiller = new RoomSlotFiller(roomLogic, _gameData.RoomItemDatabase, progress);
            
            var employeeFactory = new EmployeeFactory(_gameData.PrefabDataBase.employeeItemPrefab);
            var company = new Company(employeeFactory, roomLogic, _saveService, progressDataAdapter);
            
            var employeeStats = uiFactory.GetEmployeeStats(canvas.transform);
            var employeeStatController = new EmployeeStatController(company, employeeStats);

            var slotSelector = new SlotSelector(localEvents);
            var employeeMovement = new EmployeeMovement(localEvents);
            
            var shop = uiFactory.GetEmployeeShopView(canvas.transform);
            var shopLogic = new EmployeeShopLogic(company, shop, roomLogic, progressDataAdapter, _saveService, localEvents);
            
            var nextStateButton = uiFactory.GetNextStateButton(canvas.transform);
            var nextStateController =
                new NextStateController(nextStateButton, _gameStateMachine, progressDataAdapter,
                    _gameData.LevelMapConfig);

            _controllers.Add(roomLogic);
            _controllers.Add(cameraLogic);
            _controllers.Add(roomVisuals);
            _controllers.Add(slotSelector);
            _controllers.Add(employeeMovement);
            _controllers.Add(company);
            _controllers.Add(roomFiller);
            _controllers.Add(employeeStatController);
            _controllers.Add(shopLogic);
            _controllers.Add(nextStateController);
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