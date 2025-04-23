using System.IO;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Utils
{
    public static class Tools
    {
        public static void SaveToJson<T>(T data, string filePath)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            File.WriteAllText(filePath, json);
        }
    }
}