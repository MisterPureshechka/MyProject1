using System.Collections.Generic;

namespace Scripts.Rooms.SlotLogic
{
    public sealed class Room
    {
        private readonly Dictionary<int, Slot> _slots = new();
        public IReadOnlyDictionary<int, Slot> Slots => _slots;
        
        public Room(int initialWidth)
        {
            for (int i = 0; i < initialWidth; i++)
            {
                AddSlot(i);
            }
        }

        public Slot GetSlot(int column)
        {
            _slots.TryGetValue(column, out var slot);
            return slot;
        }

        public bool TryGetSlot(int column, out Slot slot)
        {
            return _slots.TryGetValue(column, out slot);
        }

        public Slot AddSlot(int column)
        {
            if (_slots.TryGetValue(column, out var existing))
                return existing;

            var slot = new Slot(column);
            _slots.Add(column, slot);
            return slot;
        }
        
        public Slot GetLeftNeighbour(Slot slot)
        {
            var col = slot.Column - 1;
            return GetSlot(col);
        }

        public Slot GetRightNeighbour(Slot slot)
        {
            var col = slot.Column + 1;
            return GetSlot(col);
        }

        public (Slot left, Slot right) ExpandLeftRight()
        {
            if (_slots.Count == 0)
                throw new System.Exception("Room has no slots");

            int min = int.MaxValue;
            int max = int.MinValue;

            foreach (var col in _slots.Keys)
            {
                if (col < min) min = col;
                if (col > max) max = col;
            }

            var left  = AddSlot(min - 1);
            var right = AddSlot(max + 1);

            return (left, right);
        }
    }

}