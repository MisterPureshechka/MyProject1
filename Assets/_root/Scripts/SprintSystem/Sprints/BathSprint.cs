using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class BathSprint : Sprint<IBathTask>
    {
        private readonly IInteractiveObject _objectToInteract;
        public override bool ShouldPersistTasksOnExit => false;
        public override SprintType Type => SprintType.Shower;
        public override bool HasCatalog => false;
        public override IInteractiveObject InteractiveObject => _objectToInteract;

        public BathSprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
            _objectToInteract = objectToInteract;
        }
    }
}