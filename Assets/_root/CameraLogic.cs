using Core;
using UnityEngine;

namespace Scripts
{
    public class CameraLogic : IController
    {
        private Camera _camera;
        private Vector3 _initialPosition;
        
        private Vector3 _offset = new Vector3(0f, 1.5f, -10f);

        public CameraLogic(Camera camera, Vector3 initialPosition)
        {
            _camera = camera;
            _initialPosition = initialPosition;
            
            _camera.transform.position = _initialPosition + _offset;
        }
    }
}