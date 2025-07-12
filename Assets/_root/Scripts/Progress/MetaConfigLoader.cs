using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using Scripts.Meta;
using UnityEngine;

public static class MetaConfigLoader
{
    public static SerializedDictionary<string, Metadata> LoadFromResources()
    {
        var json = Resources.Load<TextAsset>("Meta/meta_config");
        if (json == null)
        {
            Debug.LogError("meta_config.json not found in Resources/Meta/");
            return new SerializedDictionary<string, Metadata>();
        }

        return JsonConvert.DeserializeObject<SerializedDictionary<string, Metadata>>(json.text,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
    }
}