using System;
using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.GameDev
{
    [Serializable]
    public class GameProgressData
    {
        public string GameName { get; }
        public Dictionary<string, int> CompletedByTitle { get; } = new();
        public Dictionary<DevTaskType, int> CompletedByType { get; } = new();

        public GameProgressData(string gameName)
        {
            GameName = gameName;
            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
                CompletedByType[type] = 0;
        }

        public void CompleteTask(IDevTask task)
        {
            if (CompletedByTitle.ContainsKey(task.Title))
                CompletedByTitle[task.Title]++;
            else
                CompletedByTitle[task.Title] = 1;

            // по типу
            CompletedByType[task.Type]++;
        }
    }
    
    [Serializable]
    internal class GameDevProgressSnapshot
    {
        // gameName -> snapshot
        public Dictionary<string, GameProgressSnapshot> Games = new();
        public int Version = 1;
    }

    [Serializable]
    internal class GameProgressSnapshot
    {
        public string GameName;
        public Dictionary<string, int> CompletedByTitle = new(); 
        public Dictionary<string, int> CompletedByType = new();  
    }
}