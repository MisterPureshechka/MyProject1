using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "InteractiveObjectConfig", menuName = "ScriptableObjects/InteractiveObjectConfig")]
    public class InteractiveObjectConfig : ScriptableObject
    {
        [field: SerializeField] public float AnimationSpeed { get; private set; }
    }
}