using UnityEngine;

namespace Scripts.Cat
{
    public class CatTargetPosition : MonoBehaviour
    {
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public Transform JumpToTransform { get; private set; }
    }

    public enum CatTargetPositionType
    {
        Sleep,
        Watch,
        Eat,
        
    }
}