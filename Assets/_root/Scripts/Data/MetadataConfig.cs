using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "MetadataConfig", menuName = "ScriptableObjects/MetadataConfig", order = 1)]
    public class MetadataConfig : ScriptableObject
    {
        
        [field: SerializeField] public SerializedDictionary<string, Meta.Metadata> MetaFields = new();
        
        public void SaveToJson(string filePath)
        {
            string json = JsonUtility.ToJson(this, prettyPrint: true);

            File.WriteAllText(filePath, json);

            Debug.Log($"JSON saved to {filePath}");
        }
    }
}