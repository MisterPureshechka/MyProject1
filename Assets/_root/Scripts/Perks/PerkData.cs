using System;
using System.Collections.Generic;

namespace Scripts.Perks
{
    [Serializable]
    public class PerkData
    {
        public string Name;
        public string Description;
        public Dictionary<string, float> Effects;
    }
}