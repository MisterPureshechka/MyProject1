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
        private ISideRoom[] _rooms;
        
        private readonly LayerMask _sideRoomLayerMask = LayerMask.GetMask("SideRoom");

        private Vector2 _mousePosition;
        
        private float _lastUpdateTime;
        private const float UpdateInterval = 0.1f;
        
        public SideRoomChecker(IRoomView roomView, LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _rooms = roomView.SideRooms;
            
            _localEvents.OnMousePositionChange += UpdateMousePosition;
        }

        private void UpdateMousePosition(Vector2 mousePosition)
        {
            _mousePosition = mousePosition;
            
            if (Time.time - _lastUpdateTime < UpdateInterval)
                return;

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(_mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, _sideRoomLayerMask);
            if (hit.collider != null)
            {
                var sideRoom = hit.collider.GetComponent<ISideRoom>();
                if (sideRoom != null)
                {
                    _localEvents.TriggerMouseOverSideRoom(sideRoom.IsLeftRoom);
                    Debug.Log("Навели на SideRoom: " + hit.collider.name);
                }
            }
            
        }
        
        public void CleanUp()
        {
            _localEvents.OnMousePositionChange -= UpdateMousePosition;
        }
    }
}