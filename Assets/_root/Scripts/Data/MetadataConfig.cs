using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "MetadataConfig", menuName = "ScriptableObjects/MetadataConfig", order = 1)]
    public class MetadataConfig : ScriptableObject
    {
        [field: SerializeField] public float StartFood = 10f;
        [field: SerializeField] public float StartEnergy = 10f;
    }
}