using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class ReadSprint : Sprint<IReadTask> 
    {
        private readonly IInteractiveObject _objectToInteract;
        public override bool ShouldPersistTasksOnExit => false;
        public override SprintType Type => SprintType.Read;
        public override bool HasCatalog => false;
        
        public override IInteractiveObject InteractiveObject => _objectToInteract;
        
        public ReadSprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
        }
    }
}