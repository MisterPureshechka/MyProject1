using Newtonsoft.Json;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Progress
{
    public class GameProgress
    {
        private const string ProgressKey = "GameProgress";

        public void SaveProgress(ProgressData progress)
        {
            string json = JsonConvert.SerializeObject(progress, Formatting.Indented,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            PlayerPrefs.SetString(ProgressKey, json);
            PlayerPrefs.Save();

            // Резервная копия для дебага
            Tools.SaveToJson(progress, Application.dataPath + Consts.BASE_PATH);
        }

        public ProgressData LoadProgress()
        {
            if (!PlayerPrefs.HasKey(ProgressKey))
            {
                Debug.Log("PlayerPrefs Doesn't HaveKey(ProgressKey)");
                return null;
            }
            
            try
            {
                string json = PlayerPrefs.GetString(ProgressKey);
                Debug.Log("Loaded JSON: " + json);

                return JsonConvert.DeserializeObject<ProgressData>(json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error while loading progress: {e.Message}");
                return null;
            }
        }

        public void ClearProgress()
        {
            PlayerPrefs.DeleteKey(ProgressKey);
            PlayerPrefs.Save();
        }
    }
}