using _root.Planning;
using _root.Scripts.Ui.Stats;
using Scripts.Cat;
using Scripts.EcoSystem;
using Scripts.EmployeeLogic;
using Scripts.Hero;
using Scripts.OnlineShop;
using Scripts.Perks;
using Scripts.Rooms;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "PrefabDataBase", menuName = "ScriptableObjects/PrefabDataBase")]
    public class PrefabDataBase : ScriptableObject
    {
        [field: SerializeField] public SkyView Sky;
        [field: SerializeField] public RoomView RoomPrefab;
        [field: SerializeField] public CatView CatPrefab { get; private set; }
        [field: SerializeField] public TaskPanelButtonView TaskPanelButton { get; private set; }
        [field: SerializeField] public RoadMapView RoadMapPrefab { get; private set; }
        [field: SerializeField] public LevelNodeView LevelNodePrefab { get; private set; }
        [field: SerializeField] public GameObject Hero { get; private set; }
        [field: SerializeField] public GameObject Menu { get; private set; }
        [field: SerializeField] public HomeViewOld Home { get; private set; }

        [field: SerializeField] public TaskPanelView TaskPanelPrefab { get; private set; }
        [field: SerializeField] public TaskView TaskPrefab { get; private set; }
        [field: SerializeField] public SprintView SprintPrefab { get; private set; }
        [field: SerializeField] public DevTaskCatalogue devTaskCatalogue { get; private set; }
        [field: SerializeField] public CommandPanelView CommandPanelView { get; private set; }
        [field: SerializeField] public TooltipView TooltipPrefab { get; private set; }
        [field: SerializeField] public TooltipStatItem TooltipItem { get; private set; }
        [field: SerializeField] public TopPanelButtonView OnlineShopTopPanelButton { get; private set; }
        [field: SerializeField] public ShopItemView ShopItemView { get; private set; }
        [field: SerializeField] public Sprite[] ChairSprites { get; private set; }
        [field: SerializeField] public PerksCatalogue PerksCatalogue { get; private set; }
        
        [field: SerializeField] public ConnectorView ConnectorPrefab { get; private set; }
        [field: SerializeField] public EmployeeItemView employeeItemPrefab { get; private set; }
        [field: SerializeField] public EmployeeStats EmployeeStats { get; private set; }
    }
}