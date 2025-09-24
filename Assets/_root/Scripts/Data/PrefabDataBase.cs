using Scripts.Cat;
using Scripts.Hero;
using Scripts.OnlineShop;
using Scripts.Rooms;
using Scripts.Tasks;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "PrefabDataBase", menuName = "ScriptableObjects/PrefabDataBase")]
    public class PrefabDataBase : ScriptableObject
    {
        [field: SerializeField] public CatView CatPrefab { get; private set; }
        [field: SerializeField] public TaskPanelButtonView TaskPanelButton { get; private set; }
        [field: SerializeField] public GameObject Hero { get; private set; }
        [field: SerializeField] public GameObject Menu { get; private set; }
        [field: SerializeField] public HomeView Home { get; private set; }
        
        [field: SerializeField] public TaskPanelView TaskPanelPrefab { get; private set; }
        [field: SerializeField] public TaskView TaskPrefab { get; private set; }
        [field: SerializeField] public SprintView SprintPrefab { get; private set; }
        [field: SerializeField] public DevTaskCatalogue devTaskCatalogue { get; private set; }
        [field: SerializeField] public CommandPanelView CommandPanelView { get; private set; }
        [field: SerializeField] public TooltipView TooltipPrefab { get; private set; }
        [field: SerializeField] public TooltipStatItem TooltipItem { get; private set; }
        [field: SerializeField] public ReadTaskCatalogue ReadTaskCatalogue { get; private set; }
        [field: SerializeField] public TopPanelButtonView OnlineShopTopPanelButton { get; private set; }
        [field: SerializeField] public ShopItemView ShopItemView { get; private set; }
        [field: SerializeField] public Sprite[] ChairSprites { get; private set; }
    }
}