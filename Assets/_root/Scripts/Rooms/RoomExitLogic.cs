using System.Collections.Generic;
using _root.Notification;
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

        private List<CalendarEvent> _events;

        public RoomExitLogic(GameStateMachine gameStateMachine, LocalEvents localEvents, IRoomView roomView)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _roomView = roomView;

            _localEvents.OnHeroGetExit += TryExitRoom;
            _localEvents.OnSaveComeBackAction += SaveComeBackAction;

        }

        private void TryLoadEvents()
        {
            
        }

        private void SaveComeBackAction(CalendarEvent savedEvent)
        {
            _events.Add(savedEvent);
        }

        private void TryExitRoom()
        {
            _gameStateMachine.EnterState<LoadProgressState>();
        }
        public void CleanUp()
        {
            _localEvents.OnHeroGetExit -= TryExitRoom;
            Object.Destroy(_roomView.Transform.gameObject);
        }
    }
}