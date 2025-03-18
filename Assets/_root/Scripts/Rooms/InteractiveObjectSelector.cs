using System;
using Core;
using DG.Tweening;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Rooms
{
    public class InteractiveObjectSelector : IInitialization
    {
        private readonly Camera _camera;
        private readonly InputController _inputController;
        private InteractiveObjectRegisterer _iORegisterer;

        private IInteractiveObject _currentInteractiveObject;

        private Sequence _sequence;
        
        private bool _onMouseStay = false; 

        public InteractiveObjectSelector(Camera camera, InputController inputController, InteractiveObjectRegisterer iORegisterer)
        {
            _camera = camera;
            _inputController = inputController;
            _iORegisterer = iORegisterer;

            _inputController.OnMousePositionChange += OnMouseOverIO;
        }
        
        private void OnMouseOverIO(Vector3 mousePosition)
        {
            Vector2 worldPosition = _camera.ScreenToWorldPoint(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (_iORegisterer.IsObjectRegistered(clickedObject))
                {
                    if (!_onMouseStay || _currentInteractiveObject == null || _currentInteractiveObject != _iORegisterer.GetInteractiveObject(clickedObject))
                    {
                        if (_currentInteractiveObject != null)
                        {
                            _currentInteractiveObject.OnCursorExit?.Invoke();
                        }

                        _currentInteractiveObject = _iORegisterer.GetInteractiveObject(clickedObject);
                        _currentInteractiveObject.OnCursorEnter?.Invoke();
                        _onMouseStay = true;
                    }
                }
                else
                {
                    if (_onMouseStay)
                    {
                        _currentInteractiveObject?.OnCursorExit?.Invoke();
                        _currentInteractiveObject = null;
                        _onMouseStay = false;
                    }
                }
            }
        }



        public void Initialize()
        {
            
        }
    }
}