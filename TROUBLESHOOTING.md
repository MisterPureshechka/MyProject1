# Troubleshooting Guide

## Compilation Errors: "The type or namespace name 'Config' does not exist"

### Why This Happens
This is **expected behavior** during the migration. The errors occur because:
- VSCode/IDE sees the new `using Scripts.Config;` statements
- Unity hasn't compiled the new Config namespace files yet
- The Config classes exist but aren't in Unity's compiled assembly yet

### Solution
**Simply open Unity Editor** - it will automatically:
1. Detect the new C# files
2. Compile them into the project assembly
3. Resolve all namespace references
4. Clear all compilation errors

### Steps to Resolve
1. **Open Unity Editor**
2. **Wait for compilation** (watch the progress bar in bottom-right)
3. **Check Console** for any remaining errors
4. If errors persist, try: `Assets > Reimport All`

### What If Errors Persist?

#### Check File Locations
Ensure these files exist:
```
Assets/_root/Scripts/Config/
├── JsonGameSettings.cs
├── GameSettingsLoader.cs
├── GameMetaConfigAdapter.cs
└── MilestoneRulesConfigAdapter.cs
```

#### Verify Namespace Declarations
Each Config file should start with:
```csharp
namespace Scripts.Config
{
    // class code
}
```

#### Force Recompilation
In Unity Editor:
1. Go to `Assets > Reimport All`
2. Or delete `Library/` folder and reopen Unity (nuclear option)

### Expected Compilation Flow
1. ✅ New Config files created
2. ⏳ VSCode shows errors (normal - Unity hasn't compiled yet)
3. ✅ Open Unity Editor
4. ⏳ Unity compiles new files
5. ✅ Errors disappear
6. ✅ Game runs with new JSON configuration

## Runtime Errors

### "File not found: GameSettings.json"
**Solution**: The file will be auto-created with defaults on first run.

### "Failed to deserialize settings"
**Solution**: Check `GameSettings.json` for syntax errors (missing commas, brackets, etc.)

### "NullReferenceException in GameData"
**Solution**: Ensure Unity has fully compiled. Try restarting Unity Editor.

## Testing the Migration

### Quick Test
1. Open Unity
2. Enter Play Mode
3. Check Console for: `[GameSettingsLoader] Successfully loaded settings`
4. Verify milestone generation works

### Configuration Test
1. Edit `Assets/StreamingAssets/GameSettings.json`
2. Change a value (e.g., `"BaseDays": 10`)
3. Restart game
4. Verify the change took effect

## Rollback Instructions

If you need to revert the migration:

1. **Restore Original Files**
   ```bash
   git checkout HEAD -- Assets/_root/Scripts/Data/GameData.cs
   git checkout HEAD -- Assets/_root/Scripts/SprintSystem/SprintSystem.cs
   git checkout HEAD -- Assets/_root/Scripts/SprintSystem/MileStone/MilestoneGenerator.cs
   ```

2. **Delete New Files**
   ```bash
   rm -rf Assets/_root/Scripts/Config/
   rm Assets/StreamingAssets/GameSettings.json
   rm Assets/StreamingAssets/GameSettings_README.md
   ```

3. **Reopen Unity** to recompile

## Common Questions

### Q: Why not use ScriptableObjects?
A: JSON files are easier to edit, version control, and share without Unity.

### Q: Will this affect performance?
A: No - settings are loaded once and cached. Zero runtime overhead.

### Q: Can I still use ScriptableObjects?
A: The old ScriptableObject files can be deleted once migration is confirmed working.

### Q: How do I share configurations?
A: Just share the `GameSettings.json` file - no Unity required to edit it.

## Need Help?

Check these files for more information:
- `MIGRATION_SUMMARY.md` - Complete migration details
- `Assets/StreamingAssets/GameSettings_README.md` - Configuration guide
- Unity Console - Look for `[GameSettingsLoader]` messages