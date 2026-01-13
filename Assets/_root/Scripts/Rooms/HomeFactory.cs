using Scripts.Data;
using Scripts.EcoSystem;
using Scripts.Rooms.SlotLogic;
using UnityEngine;

namespace Scripts.Rooms
{
    public class HomeFactory
    {
        private PrefabDataBase _prefabDataBase;

        public HomeFactory(PrefabDataBase prefabDataBase)
        {
            _prefabDataBase = prefabDataBase;
        }

        public HomeViewOld CreateRoom()
        {
            return Object.Instantiate(_prefabDataBase.Home).GetComponent<HomeViewOld>();
        }

        public SkyView CreateSky()
        {
            return Object.Instantiate(_prefabDataBase.Sky).GetComponent<SkyView>();
        }

        public RoomView CreateRoomView()
        {
            return Object.Instantiate(_prefabDataBase.RoomPrefab).GetComponent<RoomView>();
        }
    }
}