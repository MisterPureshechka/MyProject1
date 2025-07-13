using Core;
using DG.Tweening;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Rooms;
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
            
            var canvas = GameObject.Find("Canvas");
            _tempStat = canvas.transform.Find("TempStat").GetComponent<TextMeshProUGUI>();
            
            _cameraDefaultPosition = _camera.transform.position;
            var positionKeys = _interactiveObjectConfig.CameraSideMomePositionKeys;
            _positionKeys = new Vector2[positionKeys.Length];
            
            for (int i = 0; i < positionKeys.Length; i++)
                _positionKeys[i] = _interactiveObjectConfig.CameraSideMomePositionKeys[i];
            
            
            _localEvents.OnMouseOverKitchen += MoveCameraToKitchen;
            _localEvents.OnMouseOverToilet += MoveCameraToToilet;
        }

        private void MoveCameraToToilet()
        {
            if (_cameraState == CameraState.Toilet) return;
            
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
            if (!_isMoveFinished)
                return;

            var screenWidth = Screen.width;
            var mouseX = Input.mousePosition.x;

            switch (_cameraState)
            {
                case CameraState.Kitchen:
                    if (mouseX < screenWidth / 2f)
                        ResetCameraPos();
                    break;
                case CameraState.Toilet:
                    if (mouseX > screenWidth / 2f)
                        ResetCameraPos();
                    break;
            }
        }

        private void MoveCameraToKitchen()
        {
            if (_cameraState == CameraState.Kitchen) return;

            _isMoveFinished = false;
            _cameraState = CameraState.Kitchen;

            var targetPos = _positionKeys[1];
            var result = new Vector3(targetPos.x, targetPos.y, _cameraDefaultPosition.z);

            var config = _interactiveObjectConfig;
            _camera.transform.DOKill(); // Остановить текущую анимацию

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
            if (_cameraState == CameraState.MainRoom) return;

            _isMoveFinished = false;
            _cameraState = CameraState.MainRoom;

            var config = _interactiveObjectConfig;
            _camera.transform.DOKill(); // Остановить текущую анимацию

            _camera.transform.DOMove(_cameraDefaultPosition, config.CameraMoveDuration).SetEase(config.CameraMoveEase).OnComplete(() =>
            {
                _isMoveFinished = true;
                
            });
        }

        public void CleanUp()
        {
            _localEvents.OnMouseOverKitchen -= MoveCameraToKitchen;
            _localEvents.OnMouseOverToilet -= MoveCameraToToilet;
            _localEvents.OnMouseOverMainRoom -= ResetCameraPos;
            _camera.transform.DOKill();
        }
    }
}