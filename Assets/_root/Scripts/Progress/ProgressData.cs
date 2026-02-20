using System;
using System.Collections.Generic;
using Scripts.Tasks;
using UnityEngine.Serialization;

namespace Scripts.Progress
{
    [Serializable]
    public sealed class ProgressData
    {
        public ProjectStage Stage;
        public string CompanyName;
        public int Money;
        public int Experience;
        public int OfficeCells;

        public string CurrentRoadmapNodeId;
        public HashSet<string> CompletedRoadmapNodeIds = new HashSet<string>();

        public List<ItemProgressData> Items = new();
        public List<EmployeeProgressData> Employees = new();
        
        public int GameIndex;              
        public int CurrentMilestoneIndex; 
        public int CurrentMilestoneCount;  

        public MilestoneProgressData MilestoneProgress;

        public MilestoneResultData LastMilestoneResult;
        
        public FurnitureShopSaveData CurrentShopFurniture = new FurnitureShopSaveData();
        public EmployeeShopSaveData EmployeeShop = new EmployeeShopSaveData();
        public SkillUpgradeShopSaveData SkillUpgradeShop = new SkillUpgradeShopSaveData();
        public ReleaseResultData LastReleaseResult = new ReleaseResultData();
        public bool PendingReleaseWindow;
    }

    [Serializable]
    public class FurnitureShopSaveData
    {
        public List<string> OfferIds = new List<string>(); 
    }
    
    [Serializable]
    public class EmployeeShopSaveData
    {
        public List<EmployeeOfferSave> Offers = new List<EmployeeOfferSave>();
    }

    [Serializable]
    public class EmployeeOfferSave
    {
        public string Id;                 
        public string Name;               
        public List<SkillSave> Skills = new List<SkillSave>();
        public int Price;
    }

    [Serializable]
    public class SkillUpgradeShopSaveData
    {
        public List<SkillUpgradeOfferSave> Offers = new List<SkillUpgradeOfferSave>();
    }

    [Serializable]
    public class SkillUpgradeOfferSave
    {
        public string Id;
        public int Cost;
        public List<SkillSave> Upgrades = new List<SkillSave>(); 
    }

    [Serializable]
    public class SkillSave
    {
        public string Key;   
        public float Value;
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

    [Serializable]
    public sealed class MilestoneProgressData
    {
        public bool IsActive;
        public int MilestoneIndex;
        public int DaysLimit;
        public int DaysSpent;
        public int TotalTasks;
        public int DoneTasks;
        
        public Dictionary<DevTaskType, int> DoneTasksByType = new Dictionary<DevTaskType, int>(); // Добавить это поле
        
        public bool IsCompleted;
        public bool IsFailed;
    }

    [Serializable]
    public sealed class MilestoneResultData
    {
        public bool HasValue; 

        public int GameIndex;
        public int MilestoneIndex;

        public bool Completed;
        public bool Failed;

        public int DaysSpent;
        public int DaysLimit;

        public int DoneTasks;
        public int TotalTasks;

        public int MoneyReward;
        public int SalaryCost;    
        public int NetProfit;       
        public int MoneyTotalAfter;

        public int ExperienceReward;
        public int ExperienceTotalAfter;
    }
    
    [Serializable]
    public sealed class ReleaseResultData
    {
        public bool HasValue;

        public int GameIndex;

        public int ProgrammingScore;
        public int ArtScore;
        public int GameplayScore;
        public int SoundScore;

        public int UnitsSold;
        public int Revenue;
        public int PublisherCut;
        public int NetProfit;

        public bool AwardArt;
        public bool AwardGameplay;
        public bool AwardSound;
        public bool GameOfTheYear;
    }

}
