using System;
using Core;
using DG.Tweening;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Rooms
{
    public class InteractiveObjectSelector : IInitialization, ICleanUp
    {
        private readonly Camera _camera;
        private readonly InputController _inputController;
        private InteractiveObjectRegisterer _iORegisterer;
        private readonly LocalEvents _localEvents;

        private IInteractiveObject _currentInteractiveObject;

        private Sequence _sequence;
        
        private bool _onMouseStay = false;
        private bool _isSupported;

        public InteractiveObjectSelector(Camera camera, InputController inputController, InteractiveObjectRegisterer iORegisterer, LocalEvents localEvents)
        {
            _camera = camera;
            _inputController = inputController;
            _iORegisterer = iORegisterer;
            _localEvents = localEvents;

            _localEvents.OnMousePositionChange += OnMouseOverIO;
            _localEvents.OnMouseClickWorld += OnMouseClickIO;
        }
        
        private void OnMouseOverIO(Vector2 mousePosition)
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

        private void OnMouseClickIO(Vector2 mousePosition)
        {
            Vector2 worldPosition = _camera.ScreenToWorldPoint(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider == null)
            {
                if (_currentInteractiveObject != null)
                {
                    _currentInteractiveObject.OnCursorExit?.Invoke();
                    _currentInteractiveObject = null;
                    _onMouseStay = false;
                }
                return;
            }

            GameObject clickedObject = hit.collider.gameObject;

            if (_iORegisterer.IsObjectRegistered(clickedObject))
            {
                var interactiveObj = _iORegisterer.GetInteractiveObject(clickedObject);

                _localEvents.TriggerMouseClickedIO(interactiveObj.IOType, interactiveObj.Position);
            }
            else
            {
                _localEvents.TriggerEmptyClick();
                
                if (_currentInteractiveObject != null)
                {
                    _localEvents.TriggerEmptyClick();
                    _currentInteractiveObject.OnCursorExit?.Invoke();
                    _currentInteractiveObject = null;
                    _onMouseStay = false;
                }
            }
        }

        private void SupportedTypeListener(bool isSupported)
        {
            _isSupported = isSupported;
        }

        public void Initialize()
        {
            
        }

        public void CleanUp()
        {
            _localEvents.OnMousePositionChange -= OnMouseOverIO;
            //_localEvents.OnGetSupportedType -= SupportedTypeListener;
            _localEvents.OnMouseClickWorld -= OnMouseClickIO;
        }
    }
}