using System.Collections.Generic;
using _root.Notification;
using UnityEngine;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Utils;

namespace Scripts.Rooms
{
    public class RoomExitLogic : ICleanUp
    {
        
        private readonly GameStateMachine _gameStateMachine;
        private LocalEvents _localEvents;
        private readonly IRoomViewOLD _iRoomViewOld;
        private readonly ProgressDataAdapterOLD _progressDataAdapterOld;

        private List<CalendarEvent> _events;
        private SaveService _saveService;

        public RoomExitLogic(GameStateMachine gameStateMachine, LocalEvents localEvents, IRoomViewOLD iRoomViewOld, ProgressDataAdapterOLD progressDataAdapterOld, SaveService saveService)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _iRoomViewOld = iRoomViewOld;
            _progressDataAdapterOld = progressDataAdapterOld;
            _saveService = saveService;

        }

        
        
        public void CleanUp()
        {
            Object.Destroy(_iRoomViewOld.Transform.gameObject);
        }
    }
}