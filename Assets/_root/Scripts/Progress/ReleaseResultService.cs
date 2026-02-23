using System.Collections.Generic;
using Scripts.Config;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Progress
{
    public class ReleaseResultService
    {
        private readonly MilestoneRulesConfigAdapter _rules;
        
        private const int MinScore = 1;
        private const int MaxScore = 100;

        public ReleaseResultService(MilestoneRulesConfigAdapter rules)
        {
            _rules = rules;
        }

        public ReleaseResultData GenerateReleaseResult(ProgressData data, Dictionary<DevTaskType, int> completedTasks)
        {
            int programmingScore = CalculateScore(DevTaskType.Programming, completedTasks);
            int artScore = CalculateScore(DevTaskType.Art, completedTasks);
            int gameplayScore = CalculateScore(DevTaskType.GameDesign, completedTasks);
            int soundScore = CalculateScore(DevTaskType.SoundDesign, completedTasks);

            int typeCount = CalculateDiversity(completedTasks);
            int totalTasks = CalculateTotalTasks(completedTasks);
            
            int unitsSold = CalculateUnitsSold(totalTasks, typeCount, data);

            int revenue = unitsSold * _rules.CopyPrice;
            int publisherCut = Mathf.RoundToInt(revenue * _rules.PublisherCutPercent);
            int netProfit = revenue - publisherCut;

            bool awardArt = artScore >= 8;
            bool awardGameplay = gameplayScore >= 8;
            bool awardSound = soundScore >= 8;
            bool gameOfTheYear = (artScore + gameplayScore + soundScore) > 25 && typeCount == completedTasks.Count;

            return new ReleaseResultData
            {
                HasValue = true,
                GameIndex = data.GameIndex,
                ProgrammingScore = programmingScore,
                ArtScore = artScore,
                GameplayScore = gameplayScore,
                SoundScore = soundScore,
                UnitsSold = unitsSold,
                Revenue = revenue,
                PublisherCut = publisherCut,
                NetProfit = netProfit,
                AwardArt = awardArt,
                AwardGameplay = awardGameplay,
                AwardSound = awardSound,
                GameOfTheYear = gameOfTheYear
            };
        }

        private int CalculateScore(DevTaskType type, Dictionary<DevTaskType, int> completedTasks)
        {
            if (!completedTasks.ContainsKey(type))
                return MinScore;

            int taskCount = completedTasks[type];
            return Mathf.Clamp(taskCount + Random.Range(-1, 2), MinScore, MaxScore);
        }

        private int CalculateDiversity(Dictionary<DevTaskType, int> completedTasks)
        {
            int diversity = 0;

            foreach (var kvp in completedTasks)
            {
                if (kvp.Value > 0)
                    diversity++;
            }

            return diversity;
        }

        private int CalculateTotalTasks(Dictionary<DevTaskType, int> completedTasks)
        {
            int total = 0;
            foreach (var kvp in completedTasks)
                total += kvp.Value;
            return total;
        }

        private int CalculateUnitsSold(int totalTasks, int diversity, ProgressData data)
        {
            float baseUnits = _rules.BaseUnitsSold;
            float taskMultiplier = Mathf.Pow(_rules.TaskSalesMultiplier, totalTasks);
            float diversityBonus = 1f + (diversity * 0.1f);
            float marketMultiplier = GetMarketMultiplier(data);
            
            float units = baseUnits * taskMultiplier * diversityBonus * marketMultiplier;
            return Mathf.RoundToInt(units);
        }

        private float GetMarketMultiplier(ProgressData data)
        {
            float baseMultiplier = 1.0f;

            if (data.Stage == ProjectStage.Polish)
                baseMultiplier += _rules.PolishStageMarketBonus;

            return baseMultiplier;
        }
    }
}