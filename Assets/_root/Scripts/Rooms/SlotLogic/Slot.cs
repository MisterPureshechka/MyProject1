using _root.Scripts.Rooms.RoomItems;
using Scripts.EmployeeLogic;
using Scripts.Rooms.RoomItems;

namespace Scripts.Rooms.SlotLogic
{
    public class Slot
    {
        public int Column { get; }

        public Employee Employee { get; private set; }
        public RoomItem Item { get; private set; }

        public bool IsEmpty => Employee == null && Item == null;

        public Slot(int column)
        {
            Column = column;
        }

        public void SetEmployee(Employee employee)
        {
            Clear();
            Employee = employee;
        }

        public void SetItem(RoomItem item)
        {
            Clear();
            Item = item;
        }

        public void Clear()
        {
            Employee = null;
            Item = null;
        }

        public object GetOccupant()
        {
            if (Employee != null) return Employee;
            if (Item != null) return Item;
            return null;
        }
    }
}