using System.Linq;
using Core;
using Scripts.GlobalStateMachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Rooms
{
    public class SideRoomChecker : ICleanUp
    {
        private readonly LocalEvents _localEvents;

        private Vector2 _mousePosition;
        
        private float _lastUpdateTime;
        private const float UpdateInterval = 0.1f;
        
        public SideRoomChecker(IRoomView roomView, LocalEvents localEvents)
        {
            _localEvents = localEvents;
            
            _localEvents.OnMousePositionChange += UpdateMousePosition;
        }

        private LayerMask _mainRoomLayerMask = LayerMask.GetMask("MainRoom");
        private LayerMask _kitchenLayerMask = LayerMask.GetMask("Kitchen");
        private LayerMask _toiletLayerMask = LayerMask.GetMask("Toilet");

        private string _currentRoom;

        private void UpdateMousePosition(Vector2 mousePosition)
        {
            _mousePosition = mousePosition;

            if (Time.time - _lastUpdateTime < UpdateInterval)
                return;

            _lastUpdateTime = Time.time;

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(_mousePosition);

            // Kitchen
            if (Physics2D.Raycast(worldPoint, Vector2.zero, 0f, _kitchenLayerMask).collider != null)
            {
                if (_currentRoom != "Kitchen")
                {
                    _currentRoom = "Kitchen";
                    _localEvents.TriggerMouseOverKitchen();
                    Debug.Log("Навели на Kitchen");
                }
                return;
            }

            // Toilet
            if (Physics2D.Raycast(worldPoint, Vector2.zero, 0f, _toiletLayerMask).collider != null)
            {
                if (_currentRoom != "Toilet")
                {
                    _currentRoom = "Toilet";
                    _localEvents.TriggerMouseOverToilet();
                    Debug.Log("Навели на Toilet");
                }
                return;
            }

            // MainRoom
            if (Physics2D.Raycast(worldPoint, Vector2.zero, 0f, _mainRoomLayerMask).collider != null)
            {
                if (_currentRoom != "MainRoom")
                {
                    _currentRoom = "MainRoom";
                    _localEvents.TriggerMouseOverMainRoom();
                    Debug.Log("Навели на MainRoom");
                }
                return;
            }

            // Вышли из всех
            if (_currentRoom != null)
            {
                _currentRoom = null;
                Debug.Log("Курсор вне всех комнат");
            }
        }
        
        public void CleanUp()
        {
            _localEvents.OnMousePositionChange -= UpdateMousePosition;
        }
    }
}