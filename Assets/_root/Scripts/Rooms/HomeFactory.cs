using Scripts.Data;
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

        public HomeView CreateRoom()
        {
            return Object.Instantiate(_prefabDataBase.Home).GetComponent<HomeView>();
        }
    }
}