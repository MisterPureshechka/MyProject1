using Core;
using Scripts.Meta;
using TMPro;
using UnityEngine;

namespace Scripts.Progress
{
    public class ProgressDataAdapter 
    {
        private ProgressData _progressData;

        public ProgressDataAdapter(ProgressData progressData) {
            _progressData = progressData;
        }
        
        public float GetStats(MetaType metaType)
        {
            float stats = 0f;

            foreach (var value in _progressData.Metadata.Values)
            {
                if (value.MetaType == metaType)
                {
                    stats += value.Value;
                }
            }
            
            return stats;
        }
        
        public float GetMaxStats(MetaType metaType)
        {
            float maxStats = 0f;

            foreach (var value in _progressData.Metadata.Values)
            {
                if (value.MetaType == metaType)
                {
                    maxStats += value.MaxValue;
                }
            }
            
            return maxStats;
        }
    
        public Meta.Metadata GetMetadata(string key) {
            if (_progressData.Metadata.TryGetValue(key, out var metadata)) {
                return metadata;
            }
            
            Debug.LogError($"Failed to get metadata for {key}");
            return null; 
        }
    
        public void UpdateValue(string key, float newValue) 
        {
            if (_progressData.Metadata.ContainsKey(key)) {
                
                var oldValue = _progressData.Metadata[key].Value;
                var result = oldValue + newValue;
                
                Debug.Log($"Updating {key}: old={oldValue}, delta={newValue}, result={_progressData.Metadata[key].Value}");
                
                if (result <= 0)
                {
                    _progressData.Metadata[key].Value = 0;
                    Debug.Log($" result less than 0 = {result}");
                }
                else if (result >= _progressData.Metadata[key].MaxValue)
                {
                    Debug.Log($" result more than Max = {result}");
                    _progressData.Metadata[key].Value = _progressData.Metadata[key].MaxValue;
                }
                else
                {
                    _progressData.Metadata[key].Value = result;
                }
            }
            else
            {
                Debug.LogError($"Failed to Update metadata for {key}");
            }
        }

        public ProgressData GetProgressData()
        {
            return _progressData;
        }
    }
}