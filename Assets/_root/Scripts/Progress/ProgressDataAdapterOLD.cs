using System;
using System.Collections.Generic;
using Scripts.Meta;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Progress
{
    public class ProgressDataAdapterOLD
    {
        private readonly ProgressData _progressData;
        
        private const string RoadMapProgressKey = "roadmapProgress"; 

        [Serializable]                  
        private class RoadMapProgress   
        {                               
            public List<string> СompletedNodeIds = new List<string>(); 
        }    

        public Action OnStatUpdated { get; set; }

        public ProgressDataAdapterOLD(ProgressData progressData)
        {
            // _progressData = progressData;
            // if (_progressData.ActivePerkIds == null)
            //     _progressData.ActivePerkIds = new List<string>();
            //
            // _progressData.Custom ??= new Dictionary<string, string>();
        }
        
        public List<string> GetActivePerkIds()
        {
            //eturn new List<string>(_progressData.ActivePerkIds ?? new List<string>());
            return null;
        }

        public void SetActivePerkIds(IEnumerable<string> ids)
        {
            // _progressData.ActivePerkIds ??= new List<string>();
            // _progressData.ActivePerkIds.Clear();
            // if (ids == null) return;
            // _progressData.ActivePerkIds.AddRange(ids);
        }

        public float GetStats(MetaType metaType)
        {
            float total = 0f;

            // foreach (var metadata in _progressData.Metadata.Values)
            // {
            //     if (metadata.MetaType == metaType)
            //         total += metadata.Value;
            // }

            return total;
        }

        public float GetMaxStats(MetaType metaType)
        {
            float total = 0f;

            // foreach (var metadata in _progressData.Metadata.Values)
            // {
            //     if (metadata.MetaType == metaType)
            //         total += metadata.MaxValue;
            // }

            return total;
        }

        public Meta.Metadata GetMetadata(string key)
        {
            // if (_progressData.Metadata.TryGetValue(key, out var metadata))
            //     return metadata;
            //
            // Debug.LogError($"[ProgressDataAdapter] Metadata key not found: '{key}'");
            return null;
        }

        public bool TryUpdateValue(string key, float delta)
        {
            // if (!_progressData.Metadata.TryGetValue(key, out var metadata))
            // {
            //     Debug.LogWarning($"[ProgressDataAdapter] Cannot update unknown key: '{key}'");
            //     return false;
            // }
            //
            // metadata.ChangeValue(delta); 
            // OnStatUpdated?.Invoke();

            return true;
        }
        
        public void SaveCustomJson(string key, string json)
        {
            //_progressData.Custom[key] = json;
        }

        public string LoadCustomJson(string key)
        {
            // return _progressData.Custom != null && _progressData.Custom.TryGetValue(key, out var json)
            //     ? json
            //     : null;

            return null;
        }

        public ProgressData GetProgressData() => _progressData;
        
        private RoadMapProgress LoadRoadMapProgress()
        {
            // читаем из PlayerPrefs
            string json = PlayerPrefs.GetString(RoadMapProgressKey, null);

            if (string.IsNullOrEmpty(json))
                return new RoadMapProgress();

            try
            {
                var data = JsonUtility.FromJson<RoadMapProgress>(json);
                return data ?? new RoadMapProgress();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ProgressDataAdapter] Failed to parse roadmap progress json: {e.Message}");
                return new RoadMapProgress();
            }
        }

        private void SaveRoadMapProgress(RoadMapProgress progress)
        {
            string json = JsonUtility.ToJson(progress);
            PlayerPrefs.SetString(RoadMapProgressKey, json);
            PlayerPrefs.Save();
        }

        public bool IsRoadMapNodeCompleted(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
                return false;

            var progress = LoadRoadMapProgress();
            return progress.СompletedNodeIds.Contains(nodeId);
        }

        public void MarkRoadMapNodeCompleted(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
                return;

            var progress = LoadRoadMapProgress();

            if (!progress.СompletedNodeIds.Contains(nodeId))
            {
                progress.СompletedNodeIds.Add(nodeId);
                SaveRoadMapProgress(progress);
            }
        }

        public void ClearRoadMapProgress()
        {
            SaveRoadMapProgress(new RoadMapProgress());
        }
    }
}