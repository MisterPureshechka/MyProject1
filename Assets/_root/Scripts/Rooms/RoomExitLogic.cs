using Core;
using Scripts.GlobalStateMachine;

namespace Scripts.Rooms
{
    public class RoomExitLogic : ICleanUp
    {
        private readonly GameStateMachine _gameStateMachine;
        private LocalEvents _localEvents;

        public RoomExitLogic(GameStateMachine gameStateMachine, LocalEvents localEvents)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;

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
        }
    }
}