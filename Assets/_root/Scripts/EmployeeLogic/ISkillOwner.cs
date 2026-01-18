using Scripts.Tasks;

namespace Scripts.EmployeeLogic.Scripts.EmployeeLogic
{
    public interface ISkillOwner
    {
        float MaxValue { get; }
        float GetSkill(DevTaskType type);
        void SetSkill(DevTaskType type, float value);
    }
}