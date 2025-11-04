using Scripts.Data;
using Scripts.EcoSystem;
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

        public SkyView CreateSky()
        {
            return Object.Instantiate(_prefabDataBase.Sky).GetComponent<SkyView>();
        }
    }
}