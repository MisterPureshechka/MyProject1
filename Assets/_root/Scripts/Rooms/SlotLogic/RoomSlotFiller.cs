using System;
using _root.Scripts.Rooms.RoomItems;
using Scripts.EmployeeLogic;
using Scripts.Rooms.RoomItems;
using UnityEngine;

namespace Scripts.Rooms.SlotLogic
{
    public class RoomSlotFiller
    {
        public void FillDemoLayout(RoomLogic logic, RoomItemDatabase db)
        {
            var consoleData = db.GetById("GamingConsole");
            var fridgeData  = db.GetById("Fridge");

            if (consoleData != null)
                logic.PlaceItem(0, new RoomItem(consoleData));

            if (fridgeData != null)
                logic.PlaceItem(1, new RoomItem(fridgeData));
        }
        
        public void AddEmployeeToSlot(RoomLogic logic, Employee employee, int column)
        {
            if (employee == null)
            {
                Debug.LogError($"Employee = null");
                return;
            }

            logic.PlaceItem(column, employee);
        }
    }


}