namespace Scripts.Tasks
{
    public class ChillSprint : Sprint<IChillTask> 
    {
        public override SprintType Type => SprintType.Chill;

        public ChillSprint(int capacity) : base(capacity) {}
    }
}