using Scripts.Cat;
using Scripts.Messenger;
using Scripts.Sounds;
using Scripts.Upgrade;
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
        [field: SerializeField] public MessengerConfig MessengerConfig { get; private set; }
        [field: SerializeField] public CatConfig CatConfig { get; private set; }
        [field: SerializeField] public UpgradableConfig UpgradableConfig { get; private set; }
        [field: SerializeField] public SoundConfig SoundConfig { get; private set; }
    }
}