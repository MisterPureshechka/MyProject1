using System;
using System.Collections.Generic;

namespace Scripts.Tasks
{
    [Serializable]
    public class DevSprintSaveData
    {
        public List<DevTaskSnapshot> Tasks = new(); 
    }

    [Serializable]
    public class DevTaskSnapshot
    {
        public string DevType;   
        public string Title;     
        public float Progress;
        public bool IsCompleted;
    }
}