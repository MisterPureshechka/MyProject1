using Scripts.Meta;
using UnityEngine;

namespace Scripts.Progress
{
    public class ProgressDataAdapter
    {
        private readonly ProgressData _progressData;

        public ProgressDataAdapter(ProgressData progressData)
        {
            _progressData = progressData;
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

            return true;
        }

        public ProgressData GetProgressData() => _progressData;
    }
}