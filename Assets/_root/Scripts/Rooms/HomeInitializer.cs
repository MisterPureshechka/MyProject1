using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Rooms
{
    public class HomeInitializer : IRoomInitializer
    {
        private readonly IRoomViewOLD _iRoomViewOld;

        public HomeInitializer(IRoomViewOLD iRoomViewOld)
        {
            _iRoomViewOld = iRoomViewOld;
        }

        public List<IInteractiveObject> GetAllInteravtiveObjects()
        {
            return _iRoomViewOld.InteractiveObjects;
        }

        public Vector3 GetInitialPosition()
        {
            return _iRoomViewOld.InitialPosition;
        }

        public float GetRoomSize()
        {
            return _iRoomViewOld.RoomSize;
        }
    }
}