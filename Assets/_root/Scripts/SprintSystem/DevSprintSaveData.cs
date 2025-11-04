using System;
using System.Collections.Generic;

namespace Scripts.Tasks
{
    [Serializable]
    public class DevTaskSnapshot
    {
        public string DevType;
        public string Title;
        public float  Progress;
        public bool   IsCompleted;

        public bool   IsBug;
        public bool   HasChanceForBug;
        public float  ProgressToEmitBug;
        public int    Result;

        public bool   HasProgressChanged;

        public string Id;
        public float  MaxProgress;
    }

    [Serializable]
    public class DevSprintSaveData
    {
        public List<DevTaskSnapshot> Tasks = new();
    }
}