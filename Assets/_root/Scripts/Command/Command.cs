using System;
using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class Command
    {
        public string CommandName { get; set; }
        public Action OnExecute { get; set; }
    }

    public class PurchaseCommand : Command
    {
        public int Price { get; set; }
        public InteractiveObjectType IoType; 
    }
}