using _root.Scripts.Rooms.SlotLogic.SlotSelectorFeatures;
using Scripts.Data;
using Scripts.EmployeeLogic;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Rooms.RoomItems;
using Scripts.Rooms.Scripts.Rooms;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using Scripts.Ui;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class WorkState : BaseState
    {
        public WorkState(GameStateMachine gameStateMachine, Controllers controllers, GameProgress gameProgress, GameData gameData) : base(gameStateMachine, controllers, gameProgress, gameData)
        {
        }

        public override void Enter()
        {
            var progressStat = _gameProgress.LoadProgress();
            var progressDataAdapter = new ProgressDataAdapter(progressStat);
            var localEvents = new LocalEvents();
            var camera = Camera.main;
            var homeFactory = new HomeFactory(_gameData.PrefabDataBase);
            var uiFactory = new UiFactory(_gameData);
            var canvas = Object.FindAnyObjectByType<Canvas>();

            var room = new Room(6);
            var roomView = homeFactory.CreateRoomView();
            var roomLogic = new RoomLogic(room);
            var roomItemViewFactory = new RoomItemViewFactory();
            var employeeViewFactory = new EmployeeViewFactory(_gameData.PrefabDataBase.employeeItemPrefab);
            var roomVisuals = new RoomVisuals(roomView, roomLogic, roomItemViewFactory, employeeViewFactory, localEvents);
            var cameraLogic = new CameraLogic(camera, roomVisuals.GetAverageSlotPosition());
            var roomFiller = new RoomSlotFiller();

            var sprintUi = Object.FindAnyObjectByType<SprintUI>(FindObjectsInactive.Include);
            var sprintView = uiFactory.GetSprintView(canvas.transform);
            var sprintSystem = new SprintSystem(sprintView, sprintUi, localEvents, progressDataAdapter, uiFactory);
            
            var employeeFactory = new EmployeeFactory(_gameData.PrefabDataBase.employeeItemPrefab);
            var company = new Company(employeeFactory, sprintSystem);
            
            var employeeStats = uiFactory.GetEmployeeStats(canvas.transform);
            employeeStats.Init(company);
            
            roomFiller.FillDemoLayout(roomLogic, _gameData.RoomItemDatabase);
            roomFiller.AddEmployeeToSlot(roomLogic, employeeFactory.CreateEmployee("Mike"), 3);
            roomFiller.AddEmployeeToSlot(roomLogic, employeeFactory.CreateEmployee("Andrew"), 4);

            var slotSelector = new SlotSelector(localEvents);
            var employeeMovement = new EmployeeMovement(localEvents);

            
    
            _controllers.Add(cameraLogic);
            _controllers.Add(roomVisuals);
            _controllers.Add(sprintSystem);
            _controllers.Add(slotSelector);
            _controllers.Add(employeeMovement);
            _controllers.Add(company);
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