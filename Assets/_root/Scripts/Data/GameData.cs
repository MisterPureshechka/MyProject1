using _root.Planning;
using Scripts.Cat;
using Scripts.Config;
using Scripts.Messenger;
using Scripts.Rooms.RoomItems;
using Scripts.Sounds;
using Scripts.Tasks;
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
        [field: SerializeField] public LevelMapConfig LevelMapConfig { get; private set; }
        [field: SerializeField] public RoomItemDatabase RoomItemDatabase { get; private set;}
        
        // JSON-based configuration adapters (lazy-loaded)
        private GameMetaConfigAdapter _gameMetaConfig;
        private MilestoneRulesConfigAdapter _milestoneRulesConfig;
        
        public GameMetaConfigAdapter GameMetaConfig
        {
            get
            {
                if (_gameMetaConfig == null)
                {
                    var settings = GameSettingsLoader.LoadSettings();
                    _gameMetaConfig = new GameMetaConfigAdapter(settings.GameMeta);
                }
                return _gameMetaConfig;
            }
        }
        
        public MilestoneRulesConfigAdapter MilestoneRulesConfig
        {
            get
            {
                if (_milestoneRulesConfig == null)
                {
                    var settings = GameSettingsLoader.LoadSettings();
                    _milestoneRulesConfig = new MilestoneRulesConfigAdapter(settings.MilestoneRules);
                }
                return _milestoneRulesConfig;
            }
        }
    }
}