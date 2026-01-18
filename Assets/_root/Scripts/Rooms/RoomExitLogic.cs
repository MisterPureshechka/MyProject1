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
        private readonly ProgressDataAdapter _progressDataAdapter;

        private List<CalendarEvent> _events;
        private GameProgress _gameProgress;

        public RoomExitLogic(GameStateMachine gameStateMachine, LocalEvents localEvents, IRoomViewOLD iRoomViewOld, ProgressDataAdapter progressDataAdapter, GameProgress gameProgress)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _iRoomViewOld = iRoomViewOld;
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;

        }

        
        
        public void CleanUp()
        {
            Object.Destroy(_iRoomViewOld.Transform.gameObject);
        }
    }
}