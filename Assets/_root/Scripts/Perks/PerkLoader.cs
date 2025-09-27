using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.Perks
{
    public static class PerkLoader
    {
        public static Dictionary<string, PerkData> Load()
        {
            var json = Resources.Load<TextAsset>("Meta/perks");
            if (json == null)
            {
                return new Dictionary<string, PerkData>();
            }
            return JsonConvert.DeserializeObject<Dictionary<string, PerkData>>(json.text);
        }
    }
}