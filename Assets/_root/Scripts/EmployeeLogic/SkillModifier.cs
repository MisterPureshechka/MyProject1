using Scripts.EmployeeLogic.Scripts.EmployeeLogic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.EmployeeLogic
{
    public sealed class SkillModifier
    {
        public void Add(ISkillOwner owner, DevTaskType type, float value)
        {
            if (owner == null || value == 0f)
                return;

            float next = owner.GetSkill(type) + value;
            owner.SetSkill(type, Mathf.Clamp(next, 0f, owner.MaxValue));
        }

        public void Set(ISkillOwner owner, DevTaskType type, float value)
        {
            if (owner == null)
                return;

            owner.SetSkill(type, Mathf.Clamp(value, 0f, owner.MaxValue));
        }
    }
    
    namespace Scripts.EmployeeLogic
    {
    }

}