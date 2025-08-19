using System.Collections.Generic;
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

        private readonly List<RaycastResult> _uiHits = new List<RaycastResult>(8);
        private PointerEventData _ped;

        public InputController(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }

        public void Execute(float deltatime)
        {
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
            if (IsPointerOverUI(screenPos, pointerId))
                _localEvents.TriggerMouseClickedUI(screenPos);
            else
                _localEvents.TriggerMouseClickedWorld(screenPos);
        }

        private bool IsPointerOverUI(Vector2 screenPos, int pointerId)
        {
            if (EventSystem.current == null) return false;

            _uiHits.Clear();
            _ped ??= new PointerEventData(EventSystem.current);
            _ped.Reset();
            _ped.position = screenPos;

            EventSystem.current.RaycastAll(_ped, _uiHits);
            for (int i = 0; i < _uiHits.Count; i++)
            {
                if (_uiHits[i].module is GraphicRaycaster)
                    return true; // точно UI
            }
            return false;
        }
    }
}
