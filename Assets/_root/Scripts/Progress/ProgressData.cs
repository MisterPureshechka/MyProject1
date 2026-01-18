using System;
using System.Collections.Generic;

namespace Scripts.Progress
{
    [Serializable]
    public sealed class ProgressData
    {
        public string CompanyName;
        public int Money;
        public int Experience;

        public List<ItemProgressData> Items = new();
        public List<EmployeeProgressData> Employees = new();
    }

    [Serializable]
    public sealed class ItemProgressData
    {
        public int Column;
        public string ItemId; 
    }

    [Serializable]
    public sealed class EmployeeProgressData
    {
        public string Id;
        public string Name;
        public int Column;

        public Dictionary<string, float> Skills = new();
    }
}