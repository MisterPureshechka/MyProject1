namespace Scripts.Tasks
{
    public class EatSprint : Sprint<IEatTask>
    {
        public override SprintType Type => SprintType.Eat;
        public EatSprint(int capacity) : base(capacity) { }

        public override bool ShouldPersistTasksOnExit => false;
    }
}