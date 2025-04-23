using System;

namespace Scripts.Tasks
{
    public class Command
    {
        public string CommandName { get; set; }
        public Action OnExecute { get; set; }
    }
}