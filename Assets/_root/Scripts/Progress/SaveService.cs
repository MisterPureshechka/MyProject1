using System.Collections.Generic;
using Newtonsoft.Json;
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

        private ProgressData CreateDefaultProgress()
        {
            var progress = new ProgressData
            {
                CompanyName = "New Studio",
                Money = 2500,
                Experience = 0,
                OfficeCells = 4,
                
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

            progress.Employees.Add(new EmployeeProgressData
            {
                Id = "emp_mike",
                Name = "Mike",
                Column = 1,
                Skills = new Dictionary<string, float>
                {
                    { "Programming", 3f },
                    { "Art", 2f },
                }
            });

            // --- Default item ---
            progress.Items.Add(new ItemProgressData
            {
                Column = 2,
                ItemId = "Fridge"
            });

            return progress;
        }

    }
}
