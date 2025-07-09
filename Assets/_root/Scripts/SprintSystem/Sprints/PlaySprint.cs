using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class PlaySprint : Sprint<IPlayTask>
    {
        private readonly IInteractiveObject _objectToInteract;
        public override bool ShouldPersistTasksOnExit => false;
        public override SprintType Type => SprintType.Play;
        public override bool HasCatalog => false;
        
        public override IInteractiveObject InteractiveObject => _objectToInteract;
        
        public PlaySprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
        }
    }
}