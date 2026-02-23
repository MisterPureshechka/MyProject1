using System.Collections.Generic;
using Newtonsoft.Json;
using Scripts.Config;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Progress
{
    public class SaveService
    {
        private const string ProgressKey = "GameProgress";

        public void SaveProgress(ProgressData progress)
        {
            string json = JsonConvert.SerializeObject(progress, Formatting.Indented);
            PlayerPrefs.SetString(ProgressKey, json);
            PlayerPrefs.Save();
        }

        public ProgressData LoadProgress()
        {
            if (!PlayerPrefs.HasKey(ProgressKey))
            {
                var created = CreateDefaultProgress();
                SaveProgress(created);
                return created;
            }
            
            try
            {
                string json = PlayerPrefs.GetString(ProgressKey);
                var loaded = JsonConvert.DeserializeObject<ProgressData>(json);
            
                if (loaded == null)
                {
                    var created = CreateDefaultProgress();
                    SaveProgress(created);
                    return created;
                }
            
                return loaded;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error while loading progress: {e.Message}");
                var created = CreateDefaultProgress();
                SaveProgress(created);
                return created;
            }
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(ProgressKey);
        }

        private ProgressData CreateDefaultProgress()
        {
            var settings = GameSettingsLoader.LoadSettings();
            var newGame = settings.NewGame;
            
            var progress = new ProgressData
            {
                CompanyName = newGame.CompanyName,
                Money = newGame.StartMoney,
                Experience = newGame.StartExperience,
                OfficeCells = newGame.StartOfficeCells,
                
                Stage = ProjectStage.Prototype,
                GameIndex = 0,
                CurrentMilestoneIndex = 0,
                CurrentMilestoneCount = 0,
                CurrentRoadmapNodeId = null,
                CompletedRoadmapNodeIds = new HashSet<string>(),

                MilestoneProgress = new MilestoneProgressData
                {
                    IsActive = false,
                    MilestoneIndex = 0,
                    DaysLimit = 0,
                    DaysSpent = 0,
                    TotalTasks = 0,
                    DoneTasks = 0,
                },

                LastMilestoneResult = new MilestoneResultData
                {
                    HasValue = false
                }
            };

            foreach (var emp in newGame.StartEmployees)
            {
                progress.Employees.Add(new EmployeeProgressData
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    Column = emp.Column,
                    Skills = new Dictionary<string, float>(emp.Skills)
                });
            }

            foreach (var item in newGame.StartItems)
            {
                progress.Items.Add(new ItemProgressData
                {
                    Column = item.Column,
                    ItemId = item.ItemId
                });
            }

            return progress;
        }
    }
}
