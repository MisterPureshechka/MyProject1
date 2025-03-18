using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Rooms
{
    public class HomeInitializer : IRoomInitializer
    {
        private readonly IRoomView _roomView;

        public HomeInitializer(IRoomView roomView)
        {
            _roomView = roomView;
        }

        public List<IInteractiveObject> GetAllInteravtiveObjects()
        {
            return _roomView.InteractiveObjects;
        }

        public Vector3 GetInitialPosition()
        {
            return _roomView.InitialPosition;
        }

        public float GetRoomSize()
        {
            return _roomView.RoomSize;
        }
    }
}