using Scripts.Data;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroFactory
    {
        private PrefabDataBase _prefabData;

        public HeroFactory(PrefabDataBase prefabData)
        {
            _prefabData = prefabData;
        }

        public HeroView CreateHero(Vector3 position)
        {
            var instance = Object.Instantiate(_prefabData.Hero, position, Quaternion.identity).GetComponent<HeroView>();
            
            return instance;
        }
    }
}