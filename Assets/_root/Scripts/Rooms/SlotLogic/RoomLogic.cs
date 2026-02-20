using System;
using _root.Scripts.Rooms.RoomItems;
using Core;
using Scripts.EmployeeLogic;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Rooms.SlotLogic
{
    public class RoomLogic : IController
    {
        private readonly SaveService _saveService;
        private readonly ProgressDataAdapter _progressData;
        public event Action<Slot> SlotCreated;
        public event Action<Slot> SlotUpdated;

        public Room Room { get; }

        public RoomLogic(Room room, SaveService saveService, ProgressDataAdapter progressData)
        {
            _saveService = saveService;
            _progressData = progressData;
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

            var data = _progressData.Data;
            var itemData = new ItemProgressData
            {
                Column = column,
                ItemId = item.Id,
            };
            
            data.Items.Add(itemData);
            _saveService.SaveProgress(_progressData.Data);
        }
        
        public void PlaceItem(int column, Employee employee)
        {
            var slot = Room.GetSlot(column);
            if (slot == null) return;
            
            var data = _progressData.Data;
            var employeeData = new EmployeeProgressData
            {
                Column = column,
                Name = employee.Name,
                Id = employee.Id,
                Skills = employee.ExportSkills()
            };

            data.Employees.Add(employeeData);
            _saveService.SaveProgress(_progressData.Data);
            slot.SetEmployee(employee);
            SlotUpdated?.Invoke(slot);
        }
    }

}