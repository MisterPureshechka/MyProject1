using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Config
{
    /// <summary>
    /// Adapter that provides the same API as MilestoneRulesConfig but uses JSON settings
    /// </summary>
    public class MilestoneRulesConfigAdapter
    {
        private readonly MilestoneRulesSettings _settings;
        
        public MilestoneRulesConfigAdapter(MilestoneRulesSettings settings)
        {
            _settings = settings;
        }
        
        public float SkillToWeight => _settings.SkillToWeight;
        
        public float GetBaseWeight(DevTaskType type)
        {
            string typeName = type.ToString();
            if (_settings.BaseWeights.TryGetValue(typeName, out float weight))
            {
                return weight;
            }
            return 1.0f; // Default weight
        }
        
        public int GetMinTasks(DevTaskType type, int milestoneIndex) => 0;
        
        public int GetTaskCount(int gameIndex, int milestoneIndex)
        {
            int baseCount = 1 + milestoneIndex * 2;
            int growth = gameIndex * 1;
            return Mathf.Max(1, baseCount + growth);
        }
        
        public int GetDaysLimit(int gameIndex, int milestoneIndex)
        {
            float days =
                _settings.BaseDays +
                milestoneIndex * _settings.DaysPerMilestone +
                gameIndex * _settings.DaysPerGame;
            
            return Mathf.Max(1, Mathf.RoundToInt(days));
        }
        
        public int GetMoneyReward(int gameIndex, int milestoneIndex)
        {
            int tasks = GetTaskCount(gameIndex, milestoneIndex);
            
            int reward =
                _settings.RewardBase +
                tasks * _settings.RewardPerTask +
                gameIndex * _settings.RewardPerGame +
                milestoneIndex * _settings.RewardPerMilestone;
            
            return Mathf.Max(0, reward);
        }
        
        public int ComputeDaysLimit(int baseDays, int gameIndex, float teamSkillScore)
        {
            if (!_settings.UseAutoDays)
                return baseDays;
            
            float gameDifficulty = 1f + gameIndex * _settings.DaysPerReleasedGame;
            
            float skillMultiplier = Mathf.Clamp(
                _settings.MinDaysMultiplier + teamSkillScore * _settings.SkillToDaysK,
                _settings.MinDaysMultiplier,
                _settings.MaxDaysMultiplier);
            
            float days = baseDays * gameDifficulty * skillMultiplier;
            return Mathf.Max(1, Mathf.RoundToInt(days));
        }
        
        public int ComputeMoneyReward(int baseReward, int plannedTasks, int completedTasks)
        {
            if (!_settings.UseAutoReward)
                return baseReward;
            
            float completion = plannedTasks <= 0 ? 0f : (float)completedTasks / plannedTasks;
            completion = Mathf.Clamp01(completion);
            
            float factor = Mathf.Pow(completion, _settings.CompletionPower);
            
            float reward = baseReward * factor;
            
            // минималка за частичное выполнение
            reward = Mathf.Max(reward, baseReward * _settings.MinPartialFactor * completion);
            
            return Mathf.RoundToInt(reward);
        }
    }
}