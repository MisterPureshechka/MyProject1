using Scripts.Tasks;

namespace Scripts.Perks
{
    public interface IPerkService
    {
        float GetEffectMultiplier(string effectKey);

        int ModifyMaxActiveTasks(SprintType sprint, int baseValue);
        float ModifyInterval(SprintType sprint, float baseInterval);     
        float ModifyTaskInterval(ITask task, float interval);           
        void OnSprintStart(SprintType sprint);
        void OnSprintEnd(SprintType sprint);
        void OnTaskCompleted(SprintType sprint, ITask task);
    }
}