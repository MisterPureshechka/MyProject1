namespace Scripts.Tasks
{
    public class ChillSprint : Sprint<IChillTask> 
    {
        public override bool ShouldPersistTasksOnExit => false;
        public override SprintType Type => SprintType.Chill;

        public ChillSprint(int capacity) : base(capacity) {}
        public override bool HasCatalog => false;
    }
}