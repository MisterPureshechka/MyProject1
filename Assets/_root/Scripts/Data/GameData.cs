using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
    public class GameData : ScriptableObject
    {
        [field: SerializeField] public PrefabDataBase PrefabDataBase { get; private set; }
        [field: SerializeField] public HeroConfig HeroConfig { get; private set; }
        [field: SerializeField] public InteractiveObjectConfig InteractiveObjectConfig { get; private set; }
        [field: SerializeField] public MetadataConfig MetadataConfig { get; private set; }
    }
}