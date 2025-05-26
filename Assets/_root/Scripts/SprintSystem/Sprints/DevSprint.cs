using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Tasks
{
    public class DevSprint : Sprint<IDevTask> 
    {
        private IInteractiveObject _objectToInteract;
        public override bool ShouldPersistTasksOnExit => true;
        public override SprintType Type => SprintType.Dev;
        public override bool HasCatalog => true;
        
        public override IInteractiveObject InteractiveObject => _objectToInteract;
        
        public DevSprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
            _objectToInteract = objectToInteract;
        }
    }
}