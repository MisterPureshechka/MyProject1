using DG.Tweening;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "InteractiveObjectConfig", menuName = "ScriptableObjects/InteractiveObjectConfig")]
    public class InteractiveObjectConfig : ScriptableObject
    {
        [field: SerializeField] public float AnimationSpeed { get; private set; }
        [field: SerializeField] public float CameraMoveDuration { get; private set; }
        [field: SerializeField] public Ease CameraMoveEase { get; private set; }
        [field: SerializeField] public float RoomShakePower { get; private set; }
        [field: SerializeField] public float RoomShakeDuration { get; private set; }
        [field: SerializeField] public Vector2[] CameraSideMomePositionKeys { get; private set; }
        [field: SerializeField] public Color VolumeDayColor { get; private set; }
        [field: SerializeField] public Color VolumeNightColor { get; private set; }
    }
}