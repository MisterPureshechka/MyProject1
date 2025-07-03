using Core;
using DG.Tweening;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class CameraLogic : ICleanUp
    {
        private Camera _camera;
        private readonly LocalEvents _localEvents;
        private readonly InteractiveObjectConfig _interactiveObjectConfig;

        private Vector2[] positionKeys;

        public CameraLogic(Camera camera, LocalEvents localEvents, InteractiveObjectConfig interactiveObjectConfig)
        {
            _camera = camera;
            _localEvents = localEvents;
            _interactiveObjectConfig = interactiveObjectConfig;

            positionKeys = new Vector2[_interactiveObjectConfig.CameraSideMomePositionKeys.Length + 1];
            positionKeys[_interactiveObjectConfig.CameraSideMomePositionKeys.Length] = _camera.transform.position;
            
            _localEvents.OnMouseOverSideRoom += MoveCameraAside;
        }

        private void MoveCameraAside(bool isLeftSide)
        {
            _camera.transform.DOMove(isLeftSide ? positionKeys[0] : positionKeys[1], 0.5f).SetEase(Ease.InSine);
        }

        public void CleanUp()
        {
            _localEvents.OnMouseOverSideRoom -= MoveCameraAside;
        }
    }
}