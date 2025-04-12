using System;
using System.Linq;
using Core;
using Scripts.GlobalStateMachine;
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
            
            if (IsPointerOverUIElement())
            {
                _localEvents.TriggerMouseClickedUI(mousePosition);
            }
            else
            {
                _localEvents.TriggerMouseClickedWorld(mousePosition);
            }
        }

        private bool IsPointerOverUIElement()
        {
            if (EventSystem.current == null)
                return false;
            
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            return results.Any(r => 
                r.gameObject.GetComponent<Graphic>() != null && 
                r.gameObject.GetComponent<Graphic>().raycastTarget);
        }
    }
}