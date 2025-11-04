using Core;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.ClickLogic
{
    public class ClickLogic : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private ClickState _clickState = ClickState.Room;

        public ClickLogic(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _localEvents.OnClosePanel += ChangeStateToRoom;
            _localEvents.OnOpenPanel += ChangeStateToUi;
            
            _localEvents.TriggerClickStateChange(_clickState);
        }

        private void ChangeStateToUi()
        {
            _clickState = ClickState.UI;
            _localEvents.TriggerClickStateChange(_clickState);
            Debug.Log($"Click state changed to {_clickState}");
        }
        
        private void ChangeStateToRoom()
        {
            _clickState = ClickState.Room;
            _localEvents.TriggerClickStateChange(_clickState);
            Debug.Log($"Click state changed to {_clickState}");
        }

        public void CleanUp()
        {
            _localEvents.OnClosePanel -= ChangeStateToRoom;
            _localEvents.OnOpenPanel -= ChangeStateToUi;
        }
    }
}