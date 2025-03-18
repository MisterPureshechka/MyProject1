using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "PrefabDataBase", menuName = "ScriptableObjects/PrefabDataBase")]
    public class PrefabDataBase : ScriptableObject
    {
        [field: SerializeField] public GameObject Hero { get; private set; }
        [field: SerializeField] public GameObject Menu { get; private set; }
        [field: SerializeField] public HomeView Home { get; private set; }
    }
}