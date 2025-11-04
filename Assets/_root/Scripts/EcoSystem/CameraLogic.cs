using Core;
using DG.Tweening;
using Scripts.ClickLogic;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using TMPro;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class CameraLogic : ICleanUp, IExecute
    {
        private Camera _camera;
        private readonly LocalEvents _localEvents;
        private readonly InteractiveObjectConfig _interactiveObjectConfig;

        private Vector3 _cameraDefaultPosition;
        private Vector2[] _positionKeys;
        private bool _isMoveFinished = true;
        private readonly TextMeshProUGUI _tempStat;
        private CameraState _cameraState = CameraState.MainRoom;
        private Sequence _sequence;

        private bool _isRoomClickState;
        
        private enum CameraState
        {
            MainRoom,
            Kitchen,
            Toilet
        }

        public CameraLogic(Camera camera, LocalEvents localEvents, InteractiveObjectConfig interactiveObjectConfig)
        {
            _camera = camera;
            _localEvents = localEvents;
            _interactiveObjectConfig = interactiveObjectConfig;
            
            _cameraDefaultPosition = _camera.transform.position;
            var positionKeys = _interactiveObjectConfig.CameraSideMomePositionKeys;
            _positionKeys = new Vector2[positionKeys.Length];
            
            for (int i = 0; i < positionKeys.Length; i++)
                _positionKeys[i] = new Vector2(_interactiveObjectConfig.CameraSideMomePositionKeys[i].x, _cameraDefaultPosition.y);
            
            
            // _localEvents.OnMouseOverKitchen += MoveCameraToKitchen;
            // _localEvents.OnMouseOverToilet += MoveCameraToToilet;
            // _localEvents.OnClickStateChange += ChangeClickState;
        }

        private void ChangeClickState(ClickState state)
        {
            _isRoomClickState = state == ClickState.Room;
        }

        private void MoveCameraToToilet()
        {
            if (_cameraState == CameraState.Toilet || !_isRoomClickState) return;
            
            _isMoveFinished = false;
            _cameraState = CameraState.Toilet;

            var targetPos = _positionKeys[0];
            var result = new Vector3(targetPos.x, targetPos.y, _cameraDefaultPosition.z);

            var config  = _interactiveObjectConfig;
            _camera.transform.DOMove(result, config.CameraMoveDuration).SetEase(config.CameraMoveEase).OnComplete(() =>
            {
                _isMoveFinished = true;
                
            });
        }
        
        private void HandleCameraReturnByCursor()
        {
            if (!_isMoveFinished || !_isRoomClickState)
                return;

            var screenWidth = Screen.width;
            var mouseX = Input.mousePosition.x;

            switch (_cameraState)
            {
                case CameraState.Kitchen:
                    if (mouseX < screenWidth * 0.25f)
                        ResetCameraPos();
                    break;
                case CameraState.Toilet:
                    if (mouseX > screenWidth * 0.75f)
                        ResetCameraPos();
                    break;
            }
        }

        private void MoveCameraToKitchen()
        {
            if (_cameraState == CameraState.Kitchen || !_isRoomClickState) return;

            _isMoveFinished = false;
            _cameraState = CameraState.Kitchen;

            var targetPos = _positionKeys[1];
            var result = new Vector3(targetPos.x, targetPos.y, _cameraDefaultPosition.z);

            var config = _interactiveObjectConfig;
            _camera.transform.DOKill(); 

            _camera.transform.DOMove(result, config.CameraMoveDuration).SetEase(config.CameraMoveEase).OnComplete(() =>
            {
                _isMoveFinished = true;
                
            });
        }

        public void Execute(float deltaTime)
        {
            HandleCameraReturnByCursor();
        }
        
        public void UpdateStats()
        {
            string result = "";
            result += $"_isMoveFinished = {_isMoveFinished}\n";
            _tempStat.text = result;
        }

        private void ResetCameraPos()
        {
            if (_cameraState == CameraState.MainRoom || !_isRoomClickState) return;

            _isMoveFinished = false;
            _cameraState = CameraState.MainRoom;

            var config = _interactiveObjectConfig;
            _camera.transform.DOKill(); 

            _camera.transform.DOMove(_cameraDefaultPosition, config.CameraMoveDuration).SetEase(config.CameraMoveEase).OnComplete(() =>
            {
                _isMoveFinished = true;
                
            });
        }

        public void CleanUp()
        {
            // _localEvents.OnMouseOverKitchen -= MoveCameraToKitchen;
            // _localEvents.OnMouseOverToilet -= MoveCameraToToilet;
            // _localEvents.OnMouseOverMainRoom -= ResetCameraPos;
        }
    }
}