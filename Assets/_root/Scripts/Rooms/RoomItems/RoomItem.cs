using Scripts.Rooms.RoomItems;

namespace _root.Scripts.Rooms.RoomItems
{
    public sealed class RoomItem
    {
        public RoomItemConfig Config { get; }

        public string Id => Config.Id;
        public string Name => Config.Name;
        public int Cost => Config.Cost;
        
        public RoomItemView View { get; set; }

        public RoomItem(RoomItemConfig config)
        {
            Config = config;
        }
    }
}