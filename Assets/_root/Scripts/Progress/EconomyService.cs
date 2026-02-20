using System.Collections.Generic;
using Core;
using Scripts.EmployeeLogic;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Progress
{
    public class EconomyService : IController
    {
        private const int BaseSalary = 10;
        private const float SkillSalaryFactor = 1.5f;

        private readonly Company _company;
        private readonly LocalEvents _events;
        private readonly ProgressDataAdapter _progress;
        private readonly SaveService _save;

        public EconomyService(
            ProgressDataAdapter progress,
            SaveService save,
            LocalEvents events,
            Company company)
        {
            _progress = progress;
            _save = save;
            _events = events;
            _company = company;
        }

        public void ProcessMilestoneResult(int reward)
        {
            var data = _progress.Data;

            var salaryCost = CalculateTotalSalary(_company.Employees);
            var netProfit = reward - salaryCost;

            data.Money += netProfit;

            // ВАЖНО: записываем всё, что нужно для окна
            data.LastMilestoneResult = new MilestoneResultData
            {
                HasValue = true,

                GameIndex = data.GameIndex,
                MilestoneIndex = data.CurrentMilestoneIndex,

                Completed = true,
                Failed = false,

                // если хочешь — заполни по факту
                DaysSpent = data.MilestoneProgress?.DaysSpent ?? 0,
                DaysLimit = data.MilestoneProgress?.DaysLimit ?? 0,
                DoneTasks = data.MilestoneProgress?.DoneTasks ?? 0,
                TotalTasks = data.MilestoneProgress?.TotalTasks ?? 0,

                MoneyReward = reward,
                SalaryCost = salaryCost, // <-- добавь поле в MilestoneResultData
                NetProfit = netProfit, // <-- добавь поле в MilestoneResultData
                MoneyTotalAfter = data.Money,

                ExperienceReward = 0,
                ExperienceTotalAfter = data.Experience
            };

            _save.SaveProgress(data);

            _events.TriggerWalletUpdate(data.Money);

            Debug.LogError("Time to show result sent");
            _events.TriggerMilestoneResultWindow();
        }

        private int CalculateTotalSalary(IReadOnlyList<Employee> employees)
        {
            var total = 0;

            for (var i = 0; i < employees.Count; i++)
            {
                var skillSum = 0f;
                foreach (var s in employees[i].Skills)
                    skillSum += s.Value;

                var salary = BaseSalary + (int)(skillSum * SkillSalaryFactor);
                total += salary;
            }

            return total;
        }
    }
}