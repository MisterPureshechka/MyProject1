using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.Config
{
    /// <summary>
    /// Service for loading and managing JSON-based game settings
    /// </summary>
    public class GameSettingsLoader
    {
        private const string SettingsFileName = "GameSettings.json";
        private static JsonGameSettings _cachedSettings;
        
        /// <summary>
        /// Load game settings from JSON file in StreamingAssets
        /// </summary>
        public static JsonGameSettings LoadSettings()
        {
            if (_cachedSettings != null)
                return _cachedSettings;
            
            string filePath = Path.Combine(Application.streamingAssetsPath, SettingsFileName);
            
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[GameSettingsLoader] Settings file not found at {filePath}, creating default settings");
                    _cachedSettings = CreateDefaultSettings();
                    SaveSettings(_cachedSettings);
                    return _cachedSettings;
                }
                
                string json = File.ReadAllText(filePath);
                _cachedSettings = JsonConvert.DeserializeObject<JsonGameSettings>(json);
                
                if (_cachedSettings == null)
                {
                    Debug.LogError($"[GameSettingsLoader] Failed to deserialize settings, using defaults");
                    _cachedSettings = CreateDefaultSettings();
                }
                
                Debug.Log($"[GameSettingsLoader] Successfully loaded settings from {filePath}");
                return _cachedSettings;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameSettingsLoader] Error loading settings: {e.Message}");
                _cachedSettings = CreateDefaultSettings();
                return _cachedSettings;
            }
        }
        
        /// <summary>
        /// Save settings to JSON file
        /// </summary>
        public static void SaveSettings(JsonGameSettings settings)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, SettingsFileName);
            
            try
            {
                // Ensure StreamingAssets directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(filePath, json);
                
                Debug.Log($"[GameSettingsLoader] Settings saved to {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameSettingsLoader] Error saving settings: {e.Message}");
            }
        }
        
        /// <summary>
        /// Create default settings matching the original ScriptableObject values
        /// </summary>
        private static JsonGameSettings CreateDefaultSettings()
        {
            return new JsonGameSettings
            {
                GameMeta = new GameMetaSettings
                {
                    StartMilestones = 4,
                    MaxMilestones = 8,
                    MilestonesPerGameIncrement = 1
                },
                MilestoneRules = new MilestoneRulesSettings
                {
                    SkillToWeight = 0.2f,
                    BaseDays = 6,
                    DaysPerMilestone = 1,
                    DaysPerGame = -0.3f,
                    RewardBase = 50,
                    RewardPerTask = 25,
                    RewardPerGame = 40,
                    RewardPerMilestone = 20,
                    UseAutoDays = true,
                    DaysPerReleasedGame = 0.08f,
                    SkillToDaysK = 0.02f,
                    MinDaysMultiplier = 0.60f,
                    MaxDaysMultiplier = 1.20f,
                    UseAutoReward = true,
                    CompletionPower = 1.6f,
                    MinPartialFactor = 0.10f
                }
            };
        }
        
        /// <summary>
        /// Clear cached settings (useful for testing or hot-reloading)
        /// </summary>
        public static void ClearCache()
        {
            _cachedSettings = null;
        }
    }
}