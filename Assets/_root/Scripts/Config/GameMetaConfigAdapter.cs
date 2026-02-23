using System.Collections.Generic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Config
{
    /// <summary>
    /// Adapter that provides the same API as GameMetaConfig but uses JSON settings
    /// </summary>
    public class GameMetaConfigAdapter
    {
        private readonly GameMetaSettings _settings;
        
        public GameMetaConfigAdapter(GameMetaSettings settings)
        {
            _settings = settings;
        }
        
        public int StartMilestones => _settings.StartMilestones;
        public int MaxMilestones => _settings.MaxMilestones;
        public int MilestonesPerGameIncrement => _settings.MilestonesPerGameIncrement;
        
        public int GetMilestoneCount(int gameIndex)
        {
            if (gameIndex < 0) gameIndex = 0;
            var count = _settings.StartMilestones + gameIndex * _settings.MilestonesPerGameIncrement;
            return Mathf.Clamp(count, _settings.StartMilestones, _settings.MaxMilestones);
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