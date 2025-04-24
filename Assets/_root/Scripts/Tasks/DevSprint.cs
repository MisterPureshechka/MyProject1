namespace Scripts.Tasks
{
    public class DevSprint : Sprint<IDevTask> 
    {
        public override bool ShouldPersistTasksOnExit => true;
        public override SprintType Type => SprintType.Dev;
        public DevSprint(int capacity) : base(capacity) { }
        
    }
}