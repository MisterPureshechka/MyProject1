using Scripts.Hero;
using Scripts.Rooms;
using Scripts.Ui.TaskUi;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "PrefabDataBase", menuName = "ScriptableObjects/PrefabDataBase")]
    public class PrefabDataBase : ScriptableObject
    {
        [field: SerializeField] public TaskPanelButtonView TaskPanelButton { get; private set; }
        [field: SerializeField] public GameObject Hero { get; private set; }
        [field: SerializeField] public GameObject Menu { get; private set; }
        [field: SerializeField] public HomeView Home { get; private set; }
        
        [field: SerializeField] public TaskPanelView TaskPanelPrefab { get; private set; }
        [field: SerializeField] public TaskView TaskPrefab { get; private set; }
        [field: SerializeField] public SprintView SprintPrefab { get; private set; }
    }
}