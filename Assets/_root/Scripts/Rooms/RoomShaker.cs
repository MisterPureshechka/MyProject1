using Core;
using DG.Tweening;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Rooms
{
    public class RoomShaker : ICleanUp
    {
        private readonly IRoomView _roomView;
        private LocalEvents _localEvents;
        private InteractiveObjectConfig _interactiveObjectConfig;
        private Sequence _sequence;

        public RoomShaker(IRoomView roomView, LocalEvents localEvents, InteractiveObjectConfig interactiveObjectConfig)
        {
            _roomView = roomView;
            _localEvents = localEvents;
            _interactiveObjectConfig = interactiveObjectConfig;

            _localEvents.OnNewNotificatiom += ShakeRoom;
            _localEvents.OnPurchaseUpgradeResult += TryShakeRoom;
            _localEvents.OnBugCreated += ShakeRoom;
        }

        private void TryShakeRoom(InteractiveObjectType iOType, bool succeed)
        {
            if (!succeed)
            {
                ShakeRoom();
            }
        }

        private void ShakeRoom()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            var config = _interactiveObjectConfig;

            _sequence.Append(_roomView.Transform
                .DOShakePosition(config.RoomShakeDuration, config.RoomShakePower, 50, 90f));
        }
        
        public void CleanUp()
        {
            _localEvents.OnNewNotificatiom -= ShakeRoom;
            _localEvents.OnPurchaseUpgradeResult -= TryShakeRoom;
            _localEvents.OnBugCreated -= ShakeRoom;
        }
    }
}