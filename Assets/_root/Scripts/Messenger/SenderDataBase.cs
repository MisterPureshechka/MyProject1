using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Messenger
{
    [CreateAssetMenu(menuName = "SenderDataBase", fileName = "Create SenderDataBase")]
    public class SenderDataBase : ScriptableObject
    {
        public List<SenderProfile> Profiles;

        private Dictionary<string, SenderProfile> _byId;
        private Dictionary<string, SenderProfile> _byName; 

        public void Init()
        {
            _byId = new Dictionary<string, SenderProfile>(Profiles.Count);
            _byName = new Dictionary<string, SenderProfile>(Profiles.Count, 
                System.StringComparer.OrdinalIgnoreCase);

            foreach (var p in Profiles)
            {
                if (p == null) continue;

                if (!string.IsNullOrEmpty(p.Id))
                    _byId[p.Id] = p;

                var nameKey = (p.DisplayName ?? "").Trim();
                if (!string.IsNullOrEmpty(nameKey))
                {
                    if (_byName.ContainsKey(nameKey))
                        Debug.LogWarning($"Duplicate sender name '{nameKey}' in SenderDataBase.");
                    _byName[nameKey] = p;
                }
            }
        }

        public SenderProfile GetById(string id) =>
            _byId != null && id != null && _byId.TryGetValue(id, out var p) ? p : null;

        public SenderProfile GetByName(string name) =>
            _byName != null && name != null && _byName.TryGetValue(name.Trim(), out var p) ? p : null;
    }
}