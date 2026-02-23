# GameSettings.json Configuration Guide

## Overview
This file contains all game balance and progression settings that were previously stored in ScriptableObjects. You can now edit these values directly in JSON format without needing to modify Unity assets.

## File Location
`Assets/StreamingAssets/GameSettings.json`

## Structure

### GameMeta Section
Controls milestone progression across games:
- **StartMilestones**: Initial number of milestones per game (default: 4)
- **MaxMilestones**: Maximum milestones a game can have (default: 8)
- **MilestonesPerGameIncrement**: How many milestones to add per game (default: 1)

### MilestoneRules Section
Controls task generation, difficulty, and rewards:

#### Task Distribution
- **SkillToWeight**: How much employee skills affect task distribution (default: 0.2)
- **BaseWeights**: Base weight for each task type (all default to 1.0)
  - Programming, Art, SoundDesign, GameDesign, Marketing

#### Base Days Formula
- **BaseDays**: Starting days for a milestone (default: 6)
- **DaysPerMilestone**: Days added per milestone index (default: 1)
- **DaysPerGame**: Days adjustment per game (-0.3 makes later games harder)

#### Base Reward Formula
- **RewardBase**: Base money reward (default: 50)
- **RewardPerTask**: Money per task (default: 25)
- **RewardPerGame**: Bonus per game completed (default: 40)
- **RewardPerMilestone**: Bonus per milestone index (default: 20)

#### Days Auto-Tuning
- **UseAutoDays**: Enable dynamic day calculation (default: true)
- **DaysPerReleasedGame**: Difficulty increase per game (default: 0.08)
- **SkillToDaysK**: How team skill affects days (default: 0.02)
- **MinDaysMultiplier**: Minimum days multiplier (default: 0.6)
- **MaxDaysMultiplier**: Maximum days multiplier (default: 1.2)

#### Reward Auto-Tuning
- **UseAutoReward**: Enable completion-based rewards (default: true)
- **CompletionPower**: Reward curve steepness (default: 1.6)
- **MinPartialFactor**: Minimum reward for partial completion (default: 0.1)

## How to Edit
1. Open `GameSettings.json` in any text editor
2. Modify the values you want to change
3. Save the file
4. Restart the game or reload the scene

## Migration Notes
- The old ScriptableObject configs (GameMetaConfig, MilestoneRulesConfig) are no longer used
- All values are now loaded from this JSON file at runtime
- Changes take effect immediately on game restart
- The file is automatically created with default values if missing

## Technical Details
- Loaded via `GameSettingsLoader.LoadSettings()`
- Cached after first load for performance
- Uses Newtonsoft.Json for serialization
- Adapters maintain backward compatibility with existing code