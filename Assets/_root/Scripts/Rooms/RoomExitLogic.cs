using System.Collections.Generic;
using _root.Notification;
using UnityEngine;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Job;
using Scripts.Progress;
using Scripts.Utils;

namespace Scripts.Rooms
{
    public class RoomExitLogic : ICleanUp
    {
        
        private readonly GameStateMachine _gameStateMachine;
        private LocalEvents _localEvents;
        private readonly IRoomView _roomView;
        private readonly ProgressDataAdapter _progressDataAdapter;

        private List<CalendarEvent> _events;
        private GameProgress _gameProgress;

        public RoomExitLogic(GameStateMachine gameStateMachine, LocalEvents localEvents, IRoomView roomView, ProgressDataAdapter progressDataAdapter, GameProgress gameProgress)
        {
            _gameStateMachine = gameStateMachine;
            _localEvents = localEvents;
            _roomView = roomView;
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;

            _localEvents.OnExitEventWhenExit += TryExitRoom;
        }
        
        private void TryExitRoom(ExitEvent exitEvent)
        {
            _progressDataAdapter.TryUpdateValue(Consts.GameHourKey, exitEvent?.HoursBeforeComeBack ?? 0);
            
            ChangeStatOnExit(exitEvent);
            
            _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());

            _gameStateMachine.EnterState<LoadProgressState>();
        }

        private void ChangeStatOnExit(ExitEvent exitEvent)
        {
            if (exitEvent.HealthToUpdateAfter != null)
            {
                foreach (var kv in exitEvent.HealthToUpdateAfter)
                {
                    _progressDataAdapter.TryUpdateValue(kv.Key.ToString(), kv.Value);
                }
            }
            else
            {
                Debug.LogError("exitEvent.HealthToUpdateAfter == null");
            }

            if (exitEvent.KnowledgeToUpdateAfter != null)
            {
                foreach (var kv in exitEvent.KnowledgeToUpdateAfter)
                {
                    _progressDataAdapter.TryUpdateValue(kv.Key.ToString(), kv.Value);
                }
            }
            else
            {
                Debug.LogError("exitEvent.KnowledgeToUpdateAfter == null");
            }
        }

        
        public void CleanUp()
        {
            _localEvents.OnExitEventWhenExit -= TryExitRoom;
            Object.Destroy(_roomView.Transform.gameObject);
        }
    }
}