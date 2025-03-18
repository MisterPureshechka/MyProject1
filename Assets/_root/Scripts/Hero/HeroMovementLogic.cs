using System;
using Core;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroMovementLogic : ICleanUp
    {
        public Action<Vector3> OnGetDestination;
        public Action<IInteractiveObject> OnGetTargetI0;
        

        private readonly Camera _camera;
        private readonly InteractiveObjectRegisterer _ioRegisterer;
        private readonly InputController _inputController;

        public HeroMovementLogic(Camera camera, InteractiveObjectRegisterer ioRegisterer, InputController inputController)
        {
            _camera = camera;
            _ioRegisterer = ioRegisterer;
            _inputController = inputController;

            _inputController.OnMouseClick += HandleMouseClick;
        }
        
        private void HandleMouseClick(Vector3 mousePosition)
        {
            Vector2 worldPosition = _camera.ScreenToWorldPoint(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                if (_ioRegisterer.IsObjectRegistered(clickedObject))
                {
                    IInteractiveObject io = _ioRegisterer.GetInteractiveObject(clickedObject);
                    //OnGetDestination?.Invoke(io.Position);
                    OnGetTargetI0?.Invoke(io);
                }
                else
                {
                    OnGetDestination?.Invoke(worldPosition);
                }
            }
        }

        public void CleanUp()
        {
            _inputController.OnMouseClick -= HandleMouseClick;
        }
    }
}