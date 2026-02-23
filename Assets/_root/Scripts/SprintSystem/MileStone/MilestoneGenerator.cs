using System.Collections.Generic;
using Scripts.Config;
using Scripts.EmployeeLogic;
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
            var taskTypes = GetAvailableTaskTypes(employees);
            var teamSkills = CalculateTeamSkills(taskTypes, employees);
            
            var totalTasks = CalculateTotalTasks(milestoneIndex, stage, teamSkills, rules);
            var taskCounts = DistributeTasksBySkills(totalTasks, teamSkills);
            var taskWork = rules.GetTaskWork(stage);
            
            var baseDaysLimit = rules.GetDaysLimit(gameIndex, milestoneIndex);
            var teamSkillScore = GetTotalSkillScore(teamSkills);
            var daysLimit = rules.ComputeDaysLimit(baseDaysLimit, gameIndex, teamSkillScore);
            
            var baseMoneyReward = rules.GetMoneyReward(gameIndex, milestoneIndex);
            
            var tasks = CreateTasks(taskCounts, taskWork);
            ShuffleTasks(tasks);
            
            LogGeneration(gameIndex, milestoneIndex, stage, totalTasks, taskWork, taskCounts, daysLimit, baseMoneyReward);
            
            return new MilestoneRunData
            {
                MilestoneIndex = milestoneIndex,
                MilestoneCount = milestoneCount,
                DaysLimit = daysLimit,
                MoneyReward = baseMoneyReward,
                Tasks = tasks
            };
        }

        private static List<DevTaskType> GetAvailableTaskTypes(IReadOnlyList<Employee> employees)
        {
            var types = new HashSet<DevTaskType>();
            
            for (int i = 0; i < employees.Count; i++)
            {
                foreach (var skill in employees[i].Skills)
                {
                    if (skill.Value > 0f)
                        types.Add(skill.Key);
                }
            }
            
            var list = new List<DevTaskType>(types);
            list.Sort((a, b) => ((int)a).CompareTo((int)b));
            return list;
        }

        private static Dictionary<DevTaskType, float> CalculateTeamSkills(
            List<DevTaskType> taskTypes,
            IReadOnlyList<Employee> employees)
        {
            var teamSkills = new Dictionary<DevTaskType, float>();
            
            foreach (var type in taskTypes)
                teamSkills[type] = 0f;
            
            for (int i = 0; i < employees.Count; i++)
            {
                foreach (var skill in employees[i].Skills)
                {
                    if (teamSkills.ContainsKey(skill.Key))
                        teamSkills[skill.Key] += skill.Value;
                }
            }
            
            return teamSkills;
        }

        private static float GetTotalSkillScore(Dictionary<DevTaskType, float> teamSkills)
        {
            float total = 0f;
            foreach (var skill in teamSkills.Values)
                total += Mathf.Max(0f, skill);
            return total;
        }

        private static int CalculateTotalTasks(
            int milestoneIndex,
            ProjectStage stage,
            Dictionary<DevTaskType, float> teamSkills,
            MilestoneRulesConfigAdapter rules)
        {
            int baseTasks = rules.BaseTaskCount;
            int tasksFromMilestone = milestoneIndex;
            int tasksFromSkills = Mathf.FloorToInt(GetTotalSkillScore(teamSkills) * rules.SkillToTaskFactor);
            
            int total = baseTasks + tasksFromMilestone + tasksFromSkills;
            
            float stageMultiplier = rules.GetStageMultiplier(stage);
            
            total = Mathf.RoundToInt(total * stageMultiplier);
            return Mathf.Max(1, total);
        }

        private static Dictionary<DevTaskType, int> DistributeTasksBySkills(
            int totalTasks,
            Dictionary<DevTaskType, float> teamSkills)
        {
            var counts = new Dictionary<DevTaskType, int>();
            var totalSkill = GetTotalSkillScore(teamSkills);
            
            // Note: MinTeamSkillThreshold is not passed here as this is a static helper method
            // If needed, it could be refactored to accept MilestoneRulesConfigAdapter
            if (totalSkill < 0.01f)
            {
                int tasksPerType = Mathf.Max(1, totalTasks / teamSkills.Count);
                foreach (var type in teamSkills.Keys)
                    counts[type] = tasksPerType;
                return counts;
            }
            
            int distributed = 0;
            var fractional = new List<(DevTaskType type, float frac)>();
            
            foreach (var skill in teamSkills)
            {
                float share = (skill.Value / totalSkill) * totalTasks;
                int taskCount = Mathf.FloorToInt(share);
                counts[skill.Key] = taskCount;
                distributed += taskCount;
                fractional.Add((skill.Key, share - taskCount));
            }
            
            int remaining = totalTasks - distributed;
            fractional.Sort((a, b) => b.frac.CompareTo(a.frac));
            
            for (int i = 0; i < remaining && i < fractional.Count; i++)
                counts[fractional[i].type]++;
            
            return counts;
        }


        private static List<DevTask> CreateTasks(Dictionary<DevTaskType, int> taskCounts, float taskWork)
        {
            var tasks = new List<DevTask>();
            
            foreach (var count in taskCounts)
            {
                for (int i = 0; i < count.Value; i++)
                {
                    tasks.Add(new DevTask(
                        null,
                        count.Key,
                        $"{count.Key} Task",
                        taskWork
                    ));
                }
            }
            
            return tasks;
        }

        private static void ShuffleTasks(List<DevTask> tasks)
        {
            for (int i = tasks.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (tasks[i], tasks[j]) = (tasks[j], tasks[i]);
            }
        }

        private static void LogGeneration(
            int gameIndex,
            int milestoneIndex,
            ProjectStage stage,
            int totalTasks,
            float taskWork,
            Dictionary<DevTaskType, int> taskCounts,
            int daysLimit,
            int moneyReward)
        {
            Debug.Log($"[GEN] game={gameIndex} milestone={milestoneIndex} stage={stage} totalTasks={totalTasks} taskWork={taskWork:F1} daysLimit={daysLimit} moneyReward={moneyReward}");
            
            foreach (var count in taskCounts)
                Debug.Log($"[GEN] type={count.Key} count={count.Value}");
        }
    }
}
