using System;
using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Config
{
    [Serializable]
    public class JsonGameSettings
    {
        public GameMetaSettings GameMeta;
        public MilestoneRulesSettings MilestoneRules;
        public NewGameSettings NewGame;
    }

    [Serializable]
    public class GameMetaSettings
    {
        public int StartMilestones = 4;
        public int MaxMilestones = 8;
        public int MilestonesPerGameIncrement = 1;
        public float SecondsPerDay = 240f;
    }

    [Serializable]
    public class MilestoneRulesSettings
    {
        public int BaseDays = 6;
        public int DaysPerMilestone = 1;
        public float DaysPerGame = -0.3f;
        
        public int RewardBase = 50;
        public int RewardPerTask = 25;
        public int RewardPerGame = 40;
        public int RewardPerMilestone = 20;
        
        public bool UseAutoDays = true;
        public float DaysPerReleasedGame = 0.08f;
        public float SkillToDaysK = 0.02f;
        public float MinDaysMultiplier = 0.60f;
        public float MaxDaysMultiplier = 1.20f;
        
        public bool UseAutoReward = true;
        public float CompletionPower = 1.6f;
        public float MinPartialFactor = 0.10f;
        
        public int BaseSalary = 10;
        public float SkillSalaryFactor = 1.5f;
        
        public int BaseUnitsSold = 100;
        public float TaskSalesMultiplier = 1.15f;
        public int CopyPrice = 10;
        public float PublisherCutPercent = 0.3f;
        
        // Task generation settings
        public int BaseTaskCount = 3;
        public float SkillToTaskFactor = 0.5f;
        public float MinTeamSkillThreshold = 0.01f;
        public float PrototypeStageMultiplier = 0.8f;
        public float ProductionStageMultiplier = 1.2f;
        public float PolishStageMultiplier = 1.5f;
        public float PrototypeTaskWork = 80f;
        public float ProductionTaskWork = 100f;
        public float PolishTaskWork = 130f;
        
        // Release sales bonuses
        public float PolishStageMarketBonus = 0.5f;
    }

    [Serializable]
    public class NewGameSettings
    {
        public string CompanyName = "New Studio";
        public int StartMoney = 2500;
        public int StartExperience = 0;
        public int StartOfficeCells = 4;
        
        public List<StartEmployeeData> StartEmployees = new List<StartEmployeeData>
        {
            new StartEmployeeData
            {
                Id = "emp_mike",
                Name = "Mike",
                Column = 1,
                Skills = new Dictionary<string, float>
                {
                    { "Programming", 3f },
                    { "Art", 2f }
                }
            }
        };
        
        public List<StartItemData> StartItems = new List<StartItemData>
        {
            new StartItemData
            {
                Column = 2,
                ItemId = "Fridge"
            }
        };
    }

    [Serializable]
    public class StartEmployeeData
    {
        public string Id;
        public string Name;
        public int Column;
        public Dictionary<string, float> Skills;
    }

    [Serializable]
    public class StartItemData
    {
        public int Column;
        public string ItemId;
    }
}