using System.Collections.Generic;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Progress
{
    public class ReleaseResultService
    {
        
        private const int MinScore = 1;
        private const int MaxScore = 100;

        // Основной метод генерации результата
        public ReleaseResultData GenerateReleaseResult(ProgressData data, Dictionary<DevTaskType, int> completedTasks)
        {
            // Подсчет оценок на основе выполненных задач
            int programmingScore = CalculateScore(DevTaskType.Programming, completedTasks);
            int artScore = CalculateScore(DevTaskType.Art, completedTasks);
            int gameplayScore = CalculateScore(DevTaskType.GameDesign, completedTasks);
            int soundScore = CalculateScore(DevTaskType.SoundDesign, completedTasks);

            // Подсчет количества выполненных типов задач
            int typeCount = CalculateDiversity(completedTasks); // Количество уникальных типов задач

            // Подсчет проданных копий
            int unitsSold = Mathf.RoundToInt((artScore + gameplayScore + soundScore) * typeCount * GetMarketMultiplier(data));

            // Подсчет доходов
            int revenue = unitsSold * 10; // Цена копии фиксирована: $10
            int publisherCut = Mathf.RoundToInt(revenue * 0.3f); // Издатель забирает 30%
            int netProfit = revenue - publisherCut;

            // Награды
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

        // Метод подсчета оценки по задачам одного типа
        private int CalculateScore(DevTaskType type, Dictionary<DevTaskType, int> completedTasks)
        {
            if (!completedTasks.ContainsKey(type))
                return MinScore; // Если задачи не выполнены, минимальная оценка

            int taskCount = completedTasks[type];
            return Mathf.Clamp(taskCount + Random.Range(-1, 2), MinScore, MaxScore);
        }

        // Расчет количества одновременно выполненных типов задач (разнообразие)
        private int CalculateDiversity(Dictionary<DevTaskType, int> completedTasks)
        {
            int diversity = 0;

            foreach (var kvp in completedTasks)
            {
                if (kvp.Value > 0)
                    diversity++;
            }

            return diversity; // Уникальные типы задач
        }

        // Множитель рыночного интереса
        private float GetMarketMultiplier(ProgressData data)
        {
            float baseMultiplier = 1.0f;

            // Учитывание прогресса в разработке
            if (data.Stage == ProjectStage.Polish)
                baseMultiplier += 0.5f;

            return baseMultiplier;
        }
    }
}