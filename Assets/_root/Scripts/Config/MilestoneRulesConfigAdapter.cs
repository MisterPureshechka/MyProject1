using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Config
{
    public class MilestoneRulesConfigAdapter
    {
        private readonly MilestoneRulesSettings _settings;
        
        public MilestoneRulesConfigAdapter(MilestoneRulesSettings settings)
        {
            _settings = settings;
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
            int tasks = 3 + milestoneIndex;
            
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
            
            reward = Mathf.Max(reward, baseReward * _settings.MinPartialFactor * completion);
            
            return Mathf.RoundToInt(reward);
        }
        
        public int BaseSalary => _settings.BaseSalary;
        public float SkillSalaryFactor => _settings.SkillSalaryFactor;
        
        public int BaseUnitsSold => _settings.BaseUnitsSold;
        public float TaskSalesMultiplier => _settings.TaskSalesMultiplier;
        public int CopyPrice => _settings.CopyPrice;
        public float PublisherCutPercent => _settings.PublisherCutPercent;
        
        public int BaseTaskCount => _settings.BaseTaskCount;
        public float SkillToTaskFactor => _settings.SkillToTaskFactor;
        public float MinTeamSkillThreshold => _settings.MinTeamSkillThreshold;
        public float PolishStageMarketBonus => _settings.PolishStageMarketBonus;
        
        public float GetStageMultiplier(ProjectStage stage)
        {
            return stage switch
            {
                ProjectStage.Prototype => _settings.PrototypeStageMultiplier,
                ProjectStage.Production => _settings.ProductionStageMultiplier,
                ProjectStage.Polish => _settings.PolishStageMultiplier,
                _ => 1f
            };
        }
        
        public float GetTaskWork(ProjectStage stage)
        {
            return stage switch
            {
                ProjectStage.Prototype => _settings.PrototypeTaskWork,
                ProjectStage.Production => _settings.ProductionTaskWork,
                ProjectStage.Polish => _settings.PolishTaskWork,
                _ => _settings.ProductionTaskWork
            };
        }
    }
}