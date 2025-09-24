using Scripts.Data;
using UnityEngine;

namespace Scripts.Cat
{
    public class CatFactory
    {
        private PrefabDataBase _prefabData;

        public CatFactory(PrefabDataBase prefabData)
        {
            _prefabData = prefabData;
        }

        public CatView CreateCat()
        {
            var instance = Object.Instantiate(_prefabData.CatPrefab).GetComponent<CatView>();

            return instance;
        }
    }
}