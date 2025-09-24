using UnityEngine;

namespace Scripts.Cat
{
    public class CatPositionRegisterer
    {
        private CatTargetPosition[] _catPositions;

        public CatPositionRegisterer()
        {
            RegisterPositions();
        }

        private void RegisterPositions()
        {
            _catPositions = Object.FindObjectsOfType<CatTargetPosition>();
            
            Debug.Log("found " + _catPositions.Length + " cat positions");
        }
        
        public CatTargetPosition[] GetPositions() => _catPositions;
    }
}