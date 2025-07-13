using System;
using System.Collections.Generic;
using Scripts.Meta;
using Scripts.Tasks;

namespace Scripts.Utils
{
    public static class MetadataExtensions
    {
        
        public static float GetValue(this Dictionary<string, Meta.Metadata> dict, string key)
        {
            if (!dict.TryGetValue(key, out var meta))
                throw new Exception($"Metadata key not found: {key}");

            return meta.Value;
        }

        public static float GetProgressDelta(this Dictionary<string, Meta.Metadata> dict, string key)
        {
            if (!dict.TryGetValue(key, out var meta))
                throw new Exception($"Metadata key not found: {key}");

            return meta.ProgressDelta;
        }

        public static string GetDisplayName(this Dictionary<string, Meta.Metadata> dict, string key)
        {
            if (!dict.TryGetValue(key, out var meta))
                throw new Exception($"Metadata key not found: {key}");

            return meta.DisplayName;
        }

        public static string GetTooltip(this Dictionary<string, Meta.Metadata> dict, string key)
        {
            if (!dict.TryGetValue(key, out var meta))
                throw new Exception($"Metadata key not found: {key}");

            return meta.Tooltip;
        }

        public static bool IsOfType(this Dictionary<string, Meta.Metadata> dict, string key, MetaType type)
        {
            if (!dict.TryGetValue(key, out var meta))
                throw new Exception($"Metadata key not found: {key}");

            return meta.MetaType == type;
        }
    }
}