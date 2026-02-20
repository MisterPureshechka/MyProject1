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
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class WorkState : BaseState
    {
        public WorkState(GameStateMachine gameStateMachine, Controllers controllers, SaveService saveService, GameData gameData, Canvas canvas) : base(gameStateMachine, controllers, saveService, gameData, canvas)
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
            var canvas = _canvas;

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
            var workApplier = new CompanyWorkApplier(company);
           
            var time = new TimeService(localEvents);
            
            var economy = new EconomyService(progressDataAdapter, _saveService, localEvents, company);
            var projectProgress = new ProjectProgressService(progressDataAdapter, _saveService, localEvents);

            var sprintUi = Object.FindAnyObjectByType<SprintUI>(FindObjectsInactive.Include);
            var sprintView = uiFactory.GetSprintView(canvas.transform);
            var sprintSystem = new SprintSystem(sprintView, time, sprintUi, localEvents, progressDataAdapter, uiFactory, _gameStateMachine, _gameData.GameMetaConfig, _gameData.MilestoneRulesConfig, company, _saveService, projectProgress, economy);

            var hud = uiFactory.GetHud(canvas.transform);
            var hudController = new HudController(hud, time, null, null, progressDataAdapter, localEvents, true, _gameStateMachine);
            
            var employeeStats = uiFactory.GetEmployeeStats(canvas.transform);
            var employeeStatController = new EmployeeStatController(company, employeeStats);

            var slotSelector = new SlotSelector(localEvents);
            var employeeMovement = new EmployeeMovement(localEvents);

            var resultWindow = uiFactory.GetResultWindow(canvas.transform);
            var result =
                new MilestoneResultController(resultWindow, progressDataAdapter, _gameStateMachine, localEvents, _saveService);

            var releaseWindow = uiFactory.GetReleaseWindow(canvas.transform);
            var releaseController  = new ReleaseResultController(releaseWindow, progressDataAdapter, localEvents, _gameStateMachine);
            

            _controllers.Add(roomLogic);
            _controllers.Add(cameraLogic);
            _controllers.Add(roomVisuals);
            _controllers.Add(economy);
            _controllers.Add(projectProgress);
            _controllers.Add(sprintSystem);
            _controllers.Add(time);
            _controllers.Add(slotSelector);
            _controllers.Add(employeeMovement);
            _controllers.Add(company);
            _controllers.Add(workApplier);
            _controllers.Add(roomFiller);
            _controllers.Add(employeeStatController);
            _controllers.Add(hudController);
            _controllers.Add(result);
            _controllers.Add(releaseController);
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