using System;
using Core;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts
{
    public class InputController : IExecute
    {
        public Action<Vector3> OnMouseClick;
        public Action<Vector3> OnMousePositionChange;
        
        public void Execute(float deltatime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
            
            UpdateMousePosition(Input.mousePosition);

        }

        // private void HandleMouseClick()
        // {
        //     Vector2 mousePosition = Input.mousePosition;
        //
        //     Vector2 worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        //
        //     RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        //     
        //     if (hit.collider != null)
        //     {
        //         GameObject clickedObject = hit.collider.gameObject;
        //         if(_ioRegisterer.IsObjectRegistered(clickedObject))
        //         {
        //             IInteractiveObject io = _ioRegisterer.GetInteractiveObject(clickedObject);
        //             OnMouseClick?.Invoke(io.Position);
        //         }
        //         else
        //         {
        //             Debug.Log($"Non-interactive object clicked at: {worldPosition}");
        //             OnMouseClick?.Invoke(worldPosition);
        //         }
        //     }
        // }

        private void UpdateMousePosition(Vector3 mousePosition)
        {
            OnMousePositionChange?.Invoke(mousePosition);
        }
        
        private void HandleMouseClick()
        {
            Vector2 mousePosition = Input.mousePosition;
            
            OnMouseClick?.Invoke(mousePosition);
        }
    }
}