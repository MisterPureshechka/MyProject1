using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Rooms.RoomItems;
using UnityEngine;

namespace Scripts.Rooms.RoomItems
{
    [CreateAssetMenu(menuName = "Rooms/RoomItemDatabase")]
    public class RoomItemDatabase : ScriptableObject
    {
        [SerializeField] private List<RoomItemConfig> _configs;

        public RoomItemConfig GetById(string id)
            => _configs.FirstOrDefault(c => c.Id == id);

        public IReadOnlyList<RoomItemConfig> All => _configs;
    }
}