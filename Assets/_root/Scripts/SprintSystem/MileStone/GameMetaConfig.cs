using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Tasks
{
    [CreateAssetMenu(fileName = "GameMetaConfig", menuName = "Configs/GameMetaConfig")]
    public sealed class GameMetaConfig : ScriptableObject
    {
        [Header("Milestones count progression")]
        [Min(1)] public int StartMilestones = 4;
        [Min(1)] public int MaxMilestones = 8;
        [Min(0)] public int MilestonesPerGameIncrement = 1;

        public int GetMilestoneCount(int gameIndex)
        {
            if (gameIndex < 0) gameIndex = 0;
            var count = StartMilestones + gameIndex * MilestonesPerGameIncrement;
            return Mathf.Clamp(count, StartMilestones, MaxMilestones);
        }

        public HashSet<DevTaskType> BuildTaskTypesFromEmployees(IReadOnlyList<EmployeeLogic.Employee> employees, int milestoneIndex)
        {
            var set = new HashSet<DevTaskType>();

            for (int i = 0; i < employees.Count; i++)
            {
                foreach (var kv in employees[i].Skills)
                {
                    if (kv.Value > 0f)
                        set.Add(kv.Key);
                }
            }

            return set;
        }
    }
}
