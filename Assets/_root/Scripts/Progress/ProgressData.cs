using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Scripts.Data;
using Scripts.Utils;

namespace Scripts.Progress
{
    [Serializable]
    public class ProgressData
    {
        public SerializedDictionary<string, Meta.Metadata> Metadata = new();
        
        public ProgressData(SerializedDictionary<string, Meta.Metadata> metadata) 
        {
            Metadata = metadata;
        }
    }
}