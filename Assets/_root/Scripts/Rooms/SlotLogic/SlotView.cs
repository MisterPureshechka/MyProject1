using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using Scripts.Rooms.RoomItems;
using UnityEngine;

namespace Scripts.Rooms.SlotLogic
{

    public class SlotView : MonoBehaviour
    {
        [SerializeField] private Transform _contentRoot;

        private RoomItemViewFactory _itemViewFactory;
        private EmployeeViewFactory _employeeViewFactory;   
        private RoomItemView _currentItemView;
        private EmployeeItemView _employeeItemView;
        private LocalEvents _localEvents;

        public Slot Slot { get; private set; }

        public void Init(Slot slot, RoomItemViewFactory itemFactory, EmployeeViewFactory employeeFactory, LocalEvents localEvents)
        {
            Slot = slot;
            _itemViewFactory = itemFactory;
            _employeeViewFactory = employeeFactory; 
            _localEvents = localEvents;
            Refresh();
        }

        public void Refresh()
        {
            DestroyCurrentView();

            if (Slot.Employee != null)
            {
                Debug.Log($"Refreshing SlotView with Employee: {Slot.Employee.Name}");
                _employeeItemView = _employeeViewFactory.Create(Slot.Employee, _contentRoot);
            }
            else if (Slot.Item != null)
            {
                Debug.Log($"Refreshing SlotView with Room Item: {Slot.Item.GetType().Name}");
                _currentItemView = _itemViewFactory.Create(Slot.Item, _contentRoot);
            }
            else
            {
                Debug.Log("Slot is empty.");
            }
        }
        
        private void DestroyCurrentView()
        {
            if (_currentItemView != null)
            {
                Destroy(_currentItemView.gameObject);
                _currentItemView = null;
            }

            if (_employeeItemView != null)
            {
                Destroy(_employeeItemView.gameObject);
                _employeeItemView = null;
            }
        }
        
        private void OnMouseDown()
        {
            if (Slot == null || Slot.IsEmpty)
            {
                Debug.LogWarning("Slot is empty or not initialized.");
                return;
            }

            if (Slot.Employee != null)
            {
                Debug.Log($"Slot clicked. Contains Employee: {Slot.Employee.Name}");
                _localEvents.TriggerEmployeeClicked(Slot.Employee);
            }
            else if (Slot.Item != null)
            {
                Debug.Log($"Slot clicked. Contains Room Item: {Slot.Item.Name}");
                _localEvents.TriggerRoomItemClicked(Slot.Item);
            }
            else
            {
                Debug.Log("Slot is empty.");
            }
        }
    }

}