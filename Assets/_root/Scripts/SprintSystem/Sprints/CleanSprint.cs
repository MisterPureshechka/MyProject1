using Scripts.Rooms;

namespace Scripts.Tasks
{
    public class CleanSprint : Sprint<ICleanTask>
    {
        private readonly IInteractiveObject _objectToInteract;
        public override bool ShouldPersistTasksOnExit => true;
        public override SprintType Type => SprintType.CleanPc;
        public override bool HasCatalog => false;
        
        public CleanSprint(int capacity, IInteractiveObject objectToInteract) : base(capacity, objectToInteract)
        {
        }
    }
}