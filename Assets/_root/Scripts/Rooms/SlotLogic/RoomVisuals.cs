using System;
using System.Collections.Generic;
using Core;
using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using Scripts.Rooms.RoomItems;
using Scripts.Rooms.SlotLogic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Rooms.Scripts.Rooms
{
    public class RoomVisuals : ICleanUp
    {
        private readonly RoomView _view;
        private readonly RoomLogic _logic;
        private readonly RoomItemViewFactory _itemViewFactory;
        private readonly EmployeeViewFactory _employeeViewFactory;
        private readonly LocalEvents _localEvents;

        private readonly Dictionary<int, SlotView> _slotViews = new();

        public RoomVisuals(RoomView view, RoomLogic logic, RoomItemViewFactory itemViewFactory, EmployeeViewFactory employeeViewFactory, LocalEvents localEvents)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _itemViewFactory = itemViewFactory ?? throw new ArgumentNullException(nameof(itemViewFactory));
            _employeeViewFactory = employeeViewFactory ?? throw new ArgumentNullException(nameof(employeeViewFactory));
            _localEvents = localEvents;

            _logic.SlotCreated += OnSlotCreated;
            _logic.SlotUpdated += OnSlotUpdated;

            foreach (var slot in _logic.Room.Slots.Values)
                OnSlotCreated(slot);
        }

        private void OnSlotCreated(Slot slot)
        {
            if (_slotViews.ContainsKey(slot.Column))
                return;

            var slotView = Object.Instantiate(_view.SlotPrefab, _view.SlotsRoot);
            slotView.transform.localPosition =
                new Vector3(slot.Column * _view.SlotSpacing, 0f, 0f);

            slotView.Init(slot, _itemViewFactory, _employeeViewFactory, _localEvents); 

            _slotViews.Add(slot.Column, slotView);
        }
        
        
        public Vector3 GetAverageSlotPosition()
        {
            if (_slotViews.Count == 0)
                return Vector3.zero;

            Vector3 totalPosition = Vector3.zero;
            foreach (var slotView in _slotViews.Values)
            {
                if (slotView != null)
                    totalPosition += slotView.transform.localPosition;
            }

            return totalPosition / _slotViews.Count;
        }

        private void OnSlotUpdated(Slot slot)
        {
            if (_slotViews.TryGetValue(slot.Column, out var view))
            {
                view.Refresh();
            }
        }

        public void CleanUp()
        {
            _logic.SlotCreated -= OnSlotCreated;
            _logic.SlotUpdated -= OnSlotUpdated;

            foreach (var kvp in _slotViews)
            {
                if (kvp.Value != null)
                    Object.Destroy(kvp.Value.gameObject);
            }

            _slotViews.Clear();
        }
    }
}