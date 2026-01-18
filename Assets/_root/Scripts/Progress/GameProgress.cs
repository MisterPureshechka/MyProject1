using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.Progress
{
    public class GameProgress
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
            // НЕТ СОХРАНЕНИЯ -> ВОЗВРАЩАЕМ ДЕФОЛТ
            if (!PlayerPrefs.HasKey(ProgressKey))
            {
                var created = CreateDefaultProgress();
                SaveProgress(created); // чтобы дальше уже было сохранение
                return created;
            }

            try
            {
                string json = PlayerPrefs.GetString(ProgressKey);
                var loaded = JsonConvert.DeserializeObject<ProgressData>(json);

                // если вдруг json битый или пустой — тоже откатываемся к дефолту
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
            var progress = new ProgressData();

            progress.Employees.Add(new EmployeeProgressData
            {
                Id = "emp_mike",
                Name = "Mike",
                Column = 1,
                Skills = new Dictionary<string, float>
                {
                    { "Programming", 5f }
                }
            });

            progress.Items.Add(new ItemProgressData
            {
                Column = 2,
                ItemId = "Fridge"
            });

            return progress;
        }
    }
}
