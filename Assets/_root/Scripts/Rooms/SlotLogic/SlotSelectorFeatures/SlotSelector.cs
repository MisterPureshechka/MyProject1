using _root.Scripts.Rooms.RoomItems;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.EmployeeLogic;
using Scripts.Rooms.RoomItems;
using UnityEngine;

namespace _root.Scripts.Rooms.SlotLogic.SlotSelectorFeatures
{
    public class SlotSelector : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private Employee _selectedEmployee;
        private RoomItem _selectedRoomItem;

        public SlotSelector(LocalEvents localEvents)
        {
            _localEvents = localEvents;
            _localEvents.OnEmployeeClicked += OnEmployeeSelected;
            _localEvents.OnRoomItemClicked += OnRoomItemSelected;
        }

        private void OnEmployeeSelected(Employee employee)
        {
            if(employee.IsBusy) return;
            _selectedEmployee = employee;
            CheckSelection();
        }

        private void OnRoomItemSelected(RoomItem roomItem)
        {
            _selectedRoomItem = roomItem;
            CheckSelection();
        }

        private void CheckSelection()
        {
            if (_selectedEmployee != null && _selectedRoomItem != null)
            {
                Debug.Log($"Employee {_selectedEmployee.Name} assigned to Slot with Item: {_selectedRoomItem.GetType().Name}");
                _localEvents.TriggerEmployeeWalkToItem(_selectedEmployee, _selectedRoomItem);
                ClearSelection();
            }
        }

        private void ClearSelection()
        {
            _selectedEmployee = null;
            _selectedRoomItem = null;
        }

        public void CleanUp()
        {
            _localEvents.OnEmployeeClicked -= OnEmployeeSelected;
            _localEvents.OnRoomItemClicked -= OnRoomItemSelected;
        }
    }
}