using System;
using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Config
{
    /// <summary>
    /// Root JSON configuration that combines GameMetaConfig and MilestoneRulesConfig
    /// </summary>
    [Serializable]
    public class JsonGameSettings
    {
        public GameMetaSettings GameMeta;
        public MilestoneRulesSettings MilestoneRules;
    }

    [Serializable]
    public class GameMetaSettings
    {
        public int StartMilestones = 4;
        public int MaxMilestones = 8;
        public int MilestonesPerGameIncrement = 1;
    }

    [Serializable]
    public class MilestoneRulesSettings
    {
        // Task distribution
        public float SkillToWeight = 0.2f;
        
        // Base weights for each task type
        public Dictionary<string, float> BaseWeights = new Dictionary<string, float>
        {
            { "Programming", 1.0f },
            { "Art", 1.0f },
            { "SoundDesign", 1.0f },
            { "GameDesign", 1.0f },
            { "Marketing", 1.0f }
        };
        
        // Base days formula
        public int BaseDays = 6;
        public int DaysPerMilestone = 1;
        public float DaysPerGame = -0.3f;
        
        // Base reward formula
        public int RewardBase = 50;
        public int RewardPerTask = 25;
        public int RewardPerGame = 40;
        public int RewardPerMilestone = 20;
        
        // Days auto-tuning
        public bool UseAutoDays = true;
        public float DaysPerReleasedGame = 0.08f;
        public float SkillToDaysK = 0.02f;
        public float MinDaysMultiplier = 0.60f;
        public float MaxDaysMultiplier = 1.20f;
        
        // Reward auto-tuning
        public bool UseAutoReward = true;
        public float CompletionPower = 1.6f;
        public float MinPartialFactor = 0.10f;
    }
}