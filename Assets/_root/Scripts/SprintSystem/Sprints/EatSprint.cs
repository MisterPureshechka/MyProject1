using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class EatSprint : Sprint<IEatTask>
    {
        private readonly IInteractiveObject _objectToInteract;
        public override SprintType Type => SprintType.Eat;
        public override bool ShouldPersistTasksOnExit => true;
        public override bool HasCatalog => true;
        public override IInteractiveObject InteractiveObject => _objectToInteract;

        public EatSprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
            _objectToInteract = objectToInteract;
        }
    }
}