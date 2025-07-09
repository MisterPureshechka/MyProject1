using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class ToiletSprint : Sprint<IToiletTask>
    {
        private readonly IInteractiveObject _objectToInteract;
        public override bool ShouldPersistTasksOnExit => false;
        public override SprintType Type => SprintType.Toilet;
        public override bool HasCatalog => false;
        
        public override IInteractiveObject InteractiveObject => _objectToInteract;
        
        public ToiletSprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
        }
    }
}