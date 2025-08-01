using System;
using System.Linq;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts
{
    public class InputController : IExecute
    {
        private readonly LocalEvents _localEvents;
        
        public InputController(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }

        public void Execute(float deltatime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
            
            UpdateMousePosition(Input.mousePosition);
        }

        private void UpdateMousePosition(Vector3 mousePosition)
        {
            _localEvents.TriggerMousePositionChange(mousePosition);
        }
        
        private void HandleMouseClick()
        {
            Vector2 mousePosition = Input.mousePosition;
            
            _localEvents.TriggerMouseClickedWorld(mousePosition);
        }
    }
}