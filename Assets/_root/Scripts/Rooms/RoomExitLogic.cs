using UnityEngine;
using Core;
using Scripts.GlobalStateMachine;

namespace Scripts.Rooms
{
    public class RoomExitLogic : ICleanUp
    {
        private readonly GameStateMachine _gameStateMachine;
        private LocalEvents _localEvents;
        private readonly IRoomView _roomView;

        public RoomExitLogic(GameStateMachine gameStateMachine, LocalEvents localEvents, IRoomView roomView)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _roomView = roomView;

            _localEvents.OnHeroGetIO += TryExitRoom;
        }

        private void TryExitRoom(InteractiveObjectType iOType)
        {
            if (iOType != InteractiveObjectType.Door) return;
            
            _gameStateMachine.EnterState<LoadProgressState>();
        }
        public void CleanUp()
        {
            _localEvents.OnHeroGetIO -= TryExitRoom;
            if(_roomView != null) Object.Destroy(_roomView.Transform.gameObject);
        }
    }
}