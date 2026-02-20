using Scripts.Rooms.RoomItems;
using UnityEngine;
using UnityEngine.Serialization;

namespace _root.Scripts.Rooms.RoomItems
{
    [CreateAssetMenu(menuName = "Rooms/RoomItemConfig")]
    public class RoomItemConfig : ScriptableObject
    {
        [Header("Data")]
        public string Id;
        public string Name;
        public int Cost;
        public string Description;
        
        public int FoodValue;
        public int EnergyValue;
        public int MoodValue;

        public float TimeToUpdateEmployeeStat;

        [Header("Visual")]
        public GameObject Preview;
        public RoomItemView Prefab;
    }

}