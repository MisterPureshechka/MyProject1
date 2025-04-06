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
            
            //что бы не потерять
            Tools.SaveToJson(progress, Application.dataPath + Consts.BASE_PATH );
        }

        public ProgressData LoadProgress()
        {
            if (PlayerPrefs.HasKey(ProgressKey))
            {
                string json = PlayerPrefs.GetString(ProgressKey);
                Debug.Log("Loaded JSON: " + json);

                return JsonConvert.DeserializeObject<ProgressData>(json, 
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }

            Debug.Log("PlayerPrefs Doesn't HaveKey(ProgressKey)");
            return null;
        }

        public void ClearProgress()
        {
            PlayerPrefs.DeleteKey(ProgressKey);
            PlayerPrefs.Save();
        }
    }
}