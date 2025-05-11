using Scripts.Tasks;

namespace Scripts.Rooms
{
    public static class IOUtils
    {
        public static InteractiveObjectType GetTargetIOBySprintType(SprintType sprintType)
        {
            switch (sprintType)
            {
                case SprintType.Dev:
                    return InteractiveObjectType.Pc;
                case SprintType.Chill:
                    return InteractiveObjectType.Chair;
                case SprintType.Eat:
                    return InteractiveObjectType.Fridge;
                default:
                    return InteractiveObjectType.None;
            }
        }
    }
}