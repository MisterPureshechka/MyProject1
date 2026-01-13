using _root.Scripts.Rooms.RoomItems;

namespace Scripts.Rooms.SlotLogic
{
    public sealed class RoomItemOccupant : ISlotOccupant
    {
        public RoomItem Item { get; }

        public RoomItemOccupant(RoomItem item)
        {
            Item = item;
        }
    }
}