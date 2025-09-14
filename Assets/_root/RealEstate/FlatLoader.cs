using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace _root.RealEstate
{
    public static class FlatLoader
    {
        public static List<Flat> LoadAll()
        {
            var json = Resources.Load<TextAsset>("Meta/flats");
            if (json == null)
            {
                Debug.LogError("flats.json not found at Resources/Meta/flats");
                return new List<Flat>();
            }

            var root = JsonConvert.DeserializeObject<FlatsRoot>(json.text);
            return root?.flats ?? new List<Flat>();
        }
    }
}