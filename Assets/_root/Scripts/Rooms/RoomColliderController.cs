using Core;
using Scripts.ClickLogic;
using Scripts.GlobalStateMachine;

namespace Scripts.Rooms
{
    public class RoomColliderController : ICleanUp
    {
        private readonly IRoomViewOLD _iRoomViewOld;
        private readonly LocalEvents _localEvents;

        public RoomColliderController(IRoomViewOLD iRoomViewOld, LocalEvents localEvents)
        {
            _iRoomViewOld = iRoomViewOld;
            _localEvents = localEvents;

            _localEvents.OnClickStateChange += ClickStateChangeListener;
        }

        private void ClickStateChangeListener(ClickState state)
        {
            if (state == ClickState.Room)
            {
                EnableColliders(true);
            }
            else
            {
                EnableColliders(false);
            }
        }

        private void EnableColliders(bool enable)
        {
            _iRoomViewOld.Collider.enabled = enable;
            foreach (var collider in _iRoomViewOld.SideRooms)
            {
                collider.Collider.enabled = enable;
            }
        }
        public void CleanUp()
        {
            _localEvents.OnClickStateChange -= ClickStateChangeListener;
        }
    }
}