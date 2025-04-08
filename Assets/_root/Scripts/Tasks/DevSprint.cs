namespace Scripts.Tasks
{
    public class DevSprint : Sprint<IDevTask> 
    {
        public override SprintType Type => SprintType.Dev;
        public DevSprint(int capacity) : base(capacity) { }
        
    }
}