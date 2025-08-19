using Core;
using Scripts.ClickLogic;
using Scripts.GlobalStateMachine;

namespace Scripts.Rooms
{
    public class RoomColliderController : ICleanUp
    {
        private readonly IRoomView _roomView;
        private readonly LocalEvents _localEvents;

        public RoomColliderController(IRoomView roomView, LocalEvents localEvents)
        {
            _roomView = roomView;
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
            _roomView.Collider.enabled = enable;
            foreach (var collider in _roomView.SideRooms)
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