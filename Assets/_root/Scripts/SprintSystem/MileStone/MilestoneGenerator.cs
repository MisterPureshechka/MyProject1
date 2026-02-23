using System.Collections.Generic;
using System.Linq;
using Scripts.EmployeeLogic;
using Scripts.Config;
using UnityEngine;

namespace Scripts.Tasks
{
    public enum ProjectStage
    {
        Prototype,
        Production,
        Polish,
        Released
    }

    public static class MilestoneGenerator
    {
        public static MilestoneRunData Generate(
            int gameIndex,
            int milestoneIndex,
            ProjectStage stage,
            GameMetaConfigAdapter meta,
            MilestoneRulesConfigAdapter rules,
            IReadOnlyList<Employee> employees
        )
        {
            var milestoneCount = meta.GetMilestoneCount(gameIndex);

            var baseDaysLimit   = rules.GetDaysLimit(gameIndex, milestoneIndex);
            var baseMoneyReward = rules.GetMoneyReward(gameIndex, milestoneIndex);
            var baseTotalTasks  = rules.GetTaskCount(gameIndex, milestoneIndex);

            var taskTypes      = ResolveTaskTypes(meta, employees, milestoneIndex);
            var teamSkill      = BuildTeamSkill(taskTypes, employees);
            var teamSkillScore = ComputeTeamSkillScore(taskTypes, teamSkill);

            // 1) Кол-во задач с учётом этапа + команды
            var totalTasks = ComputeTotalTasks(
                baseTotalTasks,
                milestoneIndex,
                employees.Count,
                teamSkillScore,
                stage
            );

            // 2) Дедлайн (можешь позже тоже завязать на stage, но пока достаточно teamSkillScore)
            var daysLimit = rules.ComputeDaysLimit(baseDaysLimit, gameIndex, teamSkillScore);

            // 3) Награда (пока базовая — финальная награда считается при закрытии майлстоуна)
            var moneyReward = baseMoneyReward;

            // 4) Распределение типов
            var weights = BuildWeights(rules, taskTypes, teamSkill, out var weightSum);
            if (weightSum <= 0.0001f)
                NormalizeWeightsFallback(taskTypes, weights, out weightSum);

            var counts = BuildCounts(rules, taskTypes, milestoneIndex, totalTasks, weights, weightSum);

            // 5) “Толщина” задач (progress) с учётом этапа
            var taskWork = ComputeTaskWork(100f, teamSkillScore, stage);

            var tasks = BuildTasks(counts, totalTasks, taskWork);

            // 6) Перемешать, чтобы не шли блоками по типам
            Shuffle(tasks);

            DebugLogSummary(
                gameIndex,
                milestoneIndex,
                stage,
                totalTasks,
                taskWork,
                taskTypes,
                teamSkill,
                weights,
                counts,
                daysLimit,
                moneyReward
            );

            return new MilestoneRunData
            {
                MilestoneIndex = milestoneIndex,
                MilestoneCount = milestoneCount,
                DaysLimit = daysLimit,
                MoneyReward = moneyReward,
                Tasks = tasks
            };
        }

        // -------------------------
        // Stage scaling
        // -------------------------

        private static int ComputeTotalTasks(
            int baseTotalTasks,
            int milestoneIndex,
            int employeeCount,
            float teamSkillScore,
            ProjectStage stage)
        {
            employeeCount = Mathf.Max(0, employeeCount);

            // базовое усложнение по прогрессу
            var milestoneBump = milestoneIndex / 2;

            // рост от сотрудников (чтобы доп. сотрудник реально ощущался как “больше работаем”)
            var employeeBump = Mathf.Max(0, employeeCount - 1) * 2;

            // мягкий бонус от скиллов
            var skillBump = Mathf.FloorToInt(teamSkillScore / 6f);

            var raw = baseTotalTasks + milestoneBump + employeeBump + skillBump;

            // ЭТАПЫ:
            // Prototype < Production < Polish
            float stageMul = stage switch
            {
                ProjectStage.Prototype   => 0.85f,
                ProjectStage.Production  => 1.15f,
                ProjectStage.Polish      => 1.45f,
                _                        => 1f
            };

            // Polish дополнительно масштабируем от силы команды (сублинейно, чтобы прогресс чувствовался)
            // sqrt даёт рост, но не взрывает числа
            if (stage == ProjectStage.Polish)
            {
                // 0..∞ -> +0..~1.0
                float polishTeamMul = 1f + Mathf.Sqrt(Mathf.Max(0f, teamSkillScore)) * 0.06f;
                polishTeamMul = Mathf.Clamp(polishTeamMul, 1f, 1.75f);
                stageMul *= polishTeamMul;
            }

            var result = Mathf.RoundToInt(raw * stageMul);

            // ограничения, чтобы не улетало
            var min = Mathf.Max(1, baseTotalTasks + milestoneBump);
            var max = min + 10 + employeeBump; // потолок “разумного” роста
            return Mathf.Clamp(result, min, max);
        }

        private static float ComputeTaskWork(float baseWork, float teamSkillScore, ProjectStage stage)
        {
            // базовая “толщина” задач по этапам
            float stageWorkMul = stage switch
            {
                ProjectStage.Prototype   => 0.90f,
                ProjectStage.Production  => 1.10f,
                ProjectStage.Polish      => 1.35f,
                _                        => 1f
            };

            // Polish: задачи становятся “толще” в зависимости от силы команды
            if (stage == ProjectStage.Polish)
            {
                // чем сильнее команда — тем больше “полировки” ожидается
                float polishWorkMul = 1f + Mathf.Sqrt(Mathf.Max(0f, teamSkillScore)) * 0.08f;
                polishWorkMul = Mathf.Clamp(polishWorkMul, 1f, 2.20f);
                stageWorkMul *= polishWorkMul;
            }

            return Mathf.Max(1f, baseWork * stageWorkMul);
        }

        private static void Shuffle(List<DevTask> tasks)
        {
            for (int i = tasks.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (tasks[i], tasks[j]) = (tasks[j], tasks[i]);
            }
        }

        // -------------------------
        // Resolve
        // -------------------------

        private static List<DevTaskType> ResolveTaskTypes(
            GameMetaConfigAdapter meta,
            IReadOnlyList<Employee> employees,
            int milestoneIndex)
        {
            var set = meta.BuildTaskTypesFromEmployees(employees, milestoneIndex);
            var list = new List<DevTaskType>(set);

            list.Sort((a, b) => ((int)a).CompareTo((int)b));
            return list;
        }

        // -------------------------
        // Team skill
        // -------------------------

        private static Dictionary<DevTaskType, float> BuildTeamSkill(
            List<DevTaskType> taskTypes,
            IReadOnlyList<Employee> employees)
        {
            var teamSkill = new Dictionary<DevTaskType, float>(taskTypes.Count);
            for (int i = 0; i < taskTypes.Count; i++)
                teamSkill[taskTypes[i]] = 0f;

            for (int e = 0; e < employees.Count; e++)
            {
                foreach (var kv in employees[e].Skills)
                {
                    if (teamSkill.ContainsKey(kv.Key))
                        teamSkill[kv.Key] += kv.Value;
                }
            }

            return teamSkill;
        }

        private static float ComputeTeamSkillScore(
            List<DevTaskType> taskTypes,
            Dictionary<DevTaskType, float> teamSkill)
        {
            float sum = 0f;
            for (int i = 0; i < taskTypes.Count; i++)
            {
                var type = taskTypes[i];
                if (teamSkill.TryGetValue(type, out var v))
                    sum += Mathf.Max(0f, v);
            }
            return sum;
        }

        // -------------------------
        // Weights
        // -------------------------

        private static Dictionary<DevTaskType, float> BuildWeights(
            MilestoneRulesConfigAdapter rules,
            List<DevTaskType> taskTypes,
            Dictionary<DevTaskType, float> teamSkill,
            out float weightSum)
        {
            var weights = new Dictionary<DevTaskType, float>(taskTypes.Count);
            weightSum = 0f;

            for (int i = 0; i < taskTypes.Count; i++)
            {
                var type = taskTypes[i];
                var baseW = rules.GetBaseWeight(type);
                var skill = teamSkill.TryGetValue(type, out var s) ? s : 0f;

                var w = baseW + skill * rules.SkillToWeight;
                w = Mathf.Max(0.0001f, w);

                weights[type] = w;
                weightSum += w;
            }

            return weights;
        }

        private static void NormalizeWeightsFallback(
            List<DevTaskType> taskTypes,
            Dictionary<DevTaskType, float> weights,
            out float weightSum)
        {
            weightSum = 0f;
            for (int i = 0; i < taskTypes.Count; i++)
            {
                weights[taskTypes[i]] = 1f;
                weightSum += 1f;
            }
        }

        // -------------------------
        // Counts allocation
        // -------------------------

        private static Dictionary<DevTaskType, int> BuildCounts(
            MilestoneRulesConfigAdapter rules,
            List<DevTaskType> taskTypes,
            int milestoneIndex,
            int totalTasks,
            Dictionary<DevTaskType, float> weights,
            float weightSum)
        {
            var counts = InitWithMinimums(rules, taskTypes, milestoneIndex, out var reserved);

            var remaining = Mathf.Max(0, totalTasks - reserved);
            if (remaining == 0)
                return counts;

            DistributeRemaining(taskTypes, remaining, weights, weightSum, counts);
            ClampCountsNonNegative(counts);

            return counts;
        }

        private static Dictionary<DevTaskType, int> InitWithMinimums(
            MilestoneRulesConfigAdapter rules,
            List<DevTaskType> taskTypes,
            int milestoneIndex,
            out int reserved)
        {
            reserved = 0;
            var counts = new Dictionary<DevTaskType, int>(taskTypes.Count);

            for (int i = 0; i < taskTypes.Count; i++)
            {
                var type = taskTypes[i];
                var min = Mathf.Max(0, rules.GetMinTasks(type, milestoneIndex));
                counts[type] = min;
                reserved += min;
            }

            return counts;
        }

        private static void DistributeRemaining(
            List<DevTaskType> taskTypes,
            int remaining,
            Dictionary<DevTaskType, float> weights,
            float weightSum,
            Dictionary<DevTaskType, int> counts)
        {
            var fractional = new List<(DevTaskType type, float frac)>(taskTypes.Count);
            var allocated = 0;

            for (int i = 0; i < taskTypes.Count; i++)
            {
                var type = taskTypes[i];
                var share = (weights[type] / weightSum) * remaining;

                var add = Mathf.FloorToInt(share);
                counts[type] += add;
                allocated += add;

                fractional.Add((type, share - add));
            }

            var left = remaining - allocated;
            fractional.Sort((a, b) => b.frac.CompareTo(a.frac));

            for (int i = 0; i < left && i < fractional.Count; i++)
                counts[fractional[i].type]++;
        }

        private static void ClampCountsNonNegative(Dictionary<DevTaskType, int> counts)
        {
            var keys = counts.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                var k = keys[i];
                if (counts[k] < 0) counts[k] = 0;
            }
        }

        // -------------------------
        // Task building
        // -------------------------

        private static List<DevTask> BuildTasks(
            Dictionary<DevTaskType, int> counts,
            int totalTasks,
            float taskWork)
        {
            var tasks = new List<DevTask>(Mathf.Max(0, totalTasks));

            var ordered = counts.Keys.ToList();
            ordered.Sort((a, b) => ((int)a).CompareTo((int)b));

            for (int t = 0; t < ordered.Count; t++)
            {
                var type = ordered[t];
                var count = Mathf.Max(0, counts[type]);

                for (int i = 0; i < count; i++)
                {
                    tasks.Add(new DevTask(
                        null,
                        type,
                        $"{type} Task",
                        taskWork
                    ));
                }
            }

            if (tasks.Count > totalTasks)
                tasks.RemoveRange(totalTasks, tasks.Count - totalTasks);

            return tasks;
        }

        // -------------------------
        // Debug
        // -------------------------

        private static void DebugLogSummary(
            int gameIndex,
            int milestoneIndex,
            ProjectStage stage,
            int totalTasks,
            float taskWork,
            List<DevTaskType> taskTypes,
            Dictionary<DevTaskType, float> teamSkill,
            Dictionary<DevTaskType, float> weights,
            Dictionary<DevTaskType, int> counts,
            int daysLimit,
            int moneyReward)
        {
            Debug.Log(
                $"[GEN] game={gameIndex} milestone={milestoneIndex} stage={stage} totalTasks={totalTasks} taskWork={taskWork:F1} " +
                $"types={taskTypes.Count} daysLimit={daysLimit} moneyReward={moneyReward}");

            for (int i = 0; i < taskTypes.Count; i++)
            {
                var t = taskTypes[i];
                var skill = teamSkill.TryGetValue(t, out var s) ? s : 0f;
                var w = weights.TryGetValue(t, out var ww) ? ww : 0f;
                var c = counts.TryGetValue(t, out var cc) ? cc : 0;
                Debug.Log($"[GEN] type={t} teamSkill={skill:F2} weight={w:F3} count={c}");
            }
        }
    }
}
