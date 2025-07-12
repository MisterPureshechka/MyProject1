using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class StatEffectLoader
{
    public static Dictionary<string, Dictionary<string, float>> Load()
    {
        var json = Resources.Load<TextAsset>("Meta/stat_effects");

        if (json == null)
        {
            Debug.LogError($"stat_effects.json not found in \"Meta/stat_effects\"");
            return new Dictionary<string, Dictionary<string, float>>();
        }

        return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(json.text);
    }
}