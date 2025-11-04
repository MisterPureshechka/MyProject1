using System.Collections.Generic;
using Core;
using Scripts.ClickLogic;
using Scripts.GlobalStateMachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts
{
    public class InputController : IExecute
    {
        private readonly LocalEvents _localEvents;

        private readonly List<RaycastResult> _uiHits = new List<RaycastResult>(8);
        
        private ClickState _clickState = ClickState.Room;


        private bool _isGameState;

        public InputController(Canvas canvas, LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _isGameState = true;
            
            _localEvents.OnClickStateChange += s => _clickState = s;
        }

        public void Execute(float deltatime)
        {
            if(!_isGameState) return;
            
            UpdateMousePosition(Input.mousePosition);

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
                HandlePointerDown(Input.mousePosition, -1);
#endif

            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                    HandlePointerDown(t.position, t.fingerId);
            }
        }

        private void UpdateMousePosition(Vector3 mousePosition)
        {
            _localEvents.TriggerMousePositionChange(mousePosition);
        }

        private void HandlePointerDown(Vector2 screenPos, int pointerId)
        {
            bool overUI = IsPointerOverUI(pointerId);
            
            if (overUI)
            {
                _localEvents.TriggerMouseClickedUI(screenPos);
            }
            else
            {
                _localEvents.TriggerMouseClickedWorld(screenPos);
            }
        }
        
        private bool IsPointerOverUI(int pointerId)
        {
            if (EventSystem.current == null) return false;

            if (pointerId >= 0)
                return EventSystem.current.IsPointerOverGameObject(pointerId);

            return EventSystem.current.IsPointerOverGameObject();
        }

    }
}
