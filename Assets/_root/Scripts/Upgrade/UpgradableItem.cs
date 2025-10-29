using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Upgrade
{
    public class UpgradableItem : IUpgradableItem
    {
        public InteractiveObjectType IOType { get; set; }
        public int Level { get; set; }
        public float Dirt {get; set;}

        public UpgradableItem(InteractiveObjectType iOType, int level, float dirt)
        {
            IOType = iOType;
            Level = level;
            Dirt = dirt;
        }
    }

    public interface IUpgradableItem
    {
        public InteractiveObjectType IOType { get; set; }
        public int Level { get; set; }
        public float Dirt { get; set; }
    }
}