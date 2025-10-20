using System;
using System.Collections.Generic;
using Scripts.Meta;
using UnityEngine;

namespace Scripts.Progress
{
    public class ProgressDataAdapter
    {
        private readonly ProgressData _progressData;

        public Action OnStatUpdated { get; set; }

        public ProgressDataAdapter(ProgressData progressData)
        {
            _progressData = progressData;
            if (_progressData.ActivePerkIds == null)
                _progressData.ActivePerkIds = new List<string>();
            
            _progressData.Custom ??= new Dictionary<string, string>();
        }
        
        public List<string> GetActivePerkIds()
        {
            return new List<string>(_progressData.ActivePerkIds ?? new List<string>());
        }

        public void SetActivePerkIds(IEnumerable<string> ids)
        {
            _progressData.ActivePerkIds ??= new List<string>();
            _progressData.ActivePerkIds.Clear();
            if (ids == null) return;
            _progressData.ActivePerkIds.AddRange(ids);
        }

        public float GetStats(MetaType metaType)
        {
            float total = 0f;

            foreach (var metadata in _progressData.Metadata.Values)
            {
                if (metadata.MetaType == metaType)
                    total += metadata.Value;
            }

            return total;
        }

        public float GetMaxStats(MetaType metaType)
        {
            float total = 0f;

            foreach (var metadata in _progressData.Metadata.Values)
            {
                if (metadata.MetaType == metaType)
                    total += metadata.MaxValue;
            }

            return total;
        }

        public Meta.Metadata GetMetadata(string key)
        {
            if (_progressData.Metadata.TryGetValue(key, out var metadata))
                return metadata;

            Debug.LogError($"[ProgressDataAdapter] Metadata key not found: '{key}'");
            return null;
        }

        public bool TryUpdateValue(string key, float delta)
        {
            if (!_progressData.Metadata.TryGetValue(key, out var metadata))
            {
                Debug.LogWarning($"[ProgressDataAdapter] Cannot update unknown key: '{key}'");
                return false;
            }
            
            metadata.ChangeValue(delta); 
            OnStatUpdated?.Invoke();

            return true;
        }
        
        public void SaveCustomJson(string key, string json)
        {
            _progressData.Custom[key] = json;
        }

        public string LoadCustomJson(string key)
        {
            return _progressData.Custom != null && _progressData.Custom.TryGetValue(key, out var json)
                ? json
                : null;
        }

        public ProgressData GetProgressData() => _progressData;
    }
}