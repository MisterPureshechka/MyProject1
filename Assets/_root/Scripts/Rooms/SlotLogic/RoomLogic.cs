using System;
using _root.Scripts.Rooms.RoomItems;
using Scripts.EmployeeLogic;
using UnityEngine;

namespace Scripts.Rooms.SlotLogic
{
    public class RoomLogic
    {
        public event Action<Slot> SlotCreated;
        public event Action<Slot> SlotUpdated;

        public Room Room { get; }

        public RoomLogic(Room room)
        {
            Room = room;

            foreach (var slot in Room.Slots.Values)
                SlotCreated?.Invoke(slot);
        }
        
        public Vector3 GetAveragePosition()
        {
            return Vector3.zero;
        }

        public void ExpandRoom()
        {
            var (left, right) = Room.ExpandLeftRight();
            SlotCreated?.Invoke(left);
            SlotCreated?.Invoke(right);
        }

        public void PlaceItem(int column, RoomItem item)
        {
            var slot = Room.GetSlot(column);
            if (slot == null) return;

            slot.SetItem(item);
            SlotUpdated?.Invoke(slot);
        }
        
        public void PlaceItem(int column, Employee employee)
        {
            var slot = Room.GetSlot(column);
            if (slot == null) return;

            slot.SetEmployee(employee);
            SlotUpdated?.Invoke(slot);
        }
    }

}