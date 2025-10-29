using System;
using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Rooms;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Upgrade
{
    public class UpgradeLogic : ICleanUp
    {
        private const float DirtThreshold = 0.5f;

        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;
        private readonly UpgradableConfig _upgradableConfig;

        private readonly Dictionary<InteractiveObjectType, UpgradableItem> _items = new();
        private readonly Dictionary<InteractiveObjectType, SpriteRenderer> _sprites = new();
        private readonly Dictionary<InteractiveObjectType, bool> _lastDirtyNotified = new();

        public UpgradeLogic(
            ProgressDataAdapter progressDataAdapter,
            LocalEvents localEvents,
            UpgradableConfig upgradableConfig,
            SpriteRenderer pcSprite,
            SpriteRenderer chairSprite)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            _upgradableConfig = upgradableConfig;

            if (pcSprite == null) Debug.LogError("[UpgradeLogic] pcSprite is null");
            if (chairSprite == null) Debug.LogError("[UpgradeLogic] chairSprite is null");

            InitItem(InteractiveObjectType.Pc, pcSprite);
            InitItem(InteractiveObjectType.Chair, chairSprite);

            _localEvents.OnUpgradeItemByType  += UpgradeItemByType;
            _localEvents.OnActiveSprintByType += UpdateCleanLevel;
            _localEvents.OnIODirty            += RefreshIODirty;
            _localEvents.OnPurchaseUpgradeResult += OnPurchaseUpgradeResult;

            EmitInitialDirtyStates();
        }
        
        private void OnPurchaseUpgradeResult(InteractiveObjectType type, bool success)
        {
            if (!success) return;
            UpgradeItemByType(type);
        }

        private void InitItem(InteractiveObjectType type, SpriteRenderer sprite)
        {
            var state = LoadItemState(type);
            var item = new UpgradableItem(type, state.Level, state.Dirt);

            _items[type] = item;
            _sprites[type] = sprite;

            sprite.sprite = GetSpriteFor(type, item.Level, item.Dirt);

            EmitUpgradeOffer(type);
        }

        private void RefreshIODirty(InteractiveObjectType iOType, bool isDirty)
        {
            if (isDirty) return;
            SetDirt(iOType, 0f);
        }

        private void SetDirt(InteractiveObjectType type, float value)
        {
            if (!_items.TryGetValue(type, out var item)) return;

            var state = LoadItemState(type);
            state.Dirt = Mathf.Clamp01(value);
            SaveItemState(type, state);

            if (_sprites.TryGetValue(type, out var sr) && sr != null)
                sr.sprite = GetSpriteFor(type, item.Level, state.Dirt);

            NotifyDirtyStateIfChanged(type);
        }

        private void EmitInitialDirtyStates()
        {
            foreach (var type in _items.Keys)
                NotifyDirtyStateIfChanged(type);
        }

        private void NotifyDirtyStateIfChanged(InteractiveObjectType type)
        {
            var state = LoadItemState(type);
            bool isDirty = state.Dirt >= DirtThreshold;

            if (!_lastDirtyNotified.TryGetValue(type, out var last) || last != isDirty)
            {
                _lastDirtyNotified[type] = isDirty;
                _localEvents.TriggerIODirty(type, isDirty);
            }
        }

        public void ChangeDirt(InteractiveObjectType type, float delta)
        {
            if (!_items.TryGetValue(type, out var item)) return;

            var state = LoadItemState(type);
            state.Dirt = Mathf.Clamp01(state.Dirt + delta);
            SaveItemState(type, state);

            if (_sprites.TryGetValue(type, out var sr) && sr != null)
                sr.sprite = GetSpriteFor(type, item.Level, state.Dirt);

            NotifyDirtyStateIfChanged(type);
        }

        private void UpdateCleanLevel(SprintType sprintType)
        {
            if (sprintType == SprintType.Dev)
            {
                ChangeDirt(InteractiveObjectType.Pc, 0.001f);
            }
        }
        
        private void UpgradeItemByType(InteractiveObjectType type)
        {
            if (!_items.TryGetValue(type, out var item)) return;

            var upgradable = _upgradableConfig.Upgradables.Find(x => x.IOType == type);
            if (upgradable == null || upgradable.UpgradebleObjectData.Count == 0) return;

            int maxLevelIndex = upgradable.UpgradebleObjectData.Count - 1;
            if (item.Level >= maxLevelIndex)
            {
                EmitUpgradeOffer(type);
                return;
            }

            item.Level++;

            var state = LoadItemState(type);
            state.Level = item.Level;
            SaveItemState(type, state);

            if (_sprites.TryGetValue(type, out var sr) && sr != null)
                sr.sprite = GetSpriteFor(type, item.Level, state.Dirt);

            EmitUpgradeOffer(type);
        }

        private bool IsUpgradeAvailable(InteractiveObjectType type)
        {
            if (!_items.TryGetValue(type, out var item)) return false;

            var upgradable = _upgradableConfig.Upgradables.Find(x => x.IOType == type);
            if (upgradable == null) return false;

            int maxLevel = upgradable.UpgradebleObjectData.Count - 1;
            return item.Level < maxLevel;
        }

        private int GetNextUpgradePrice(InteractiveObjectType type)
        {
            if (!_items.TryGetValue(type, out var item)) return 0;

            var upgradable = _upgradableConfig.Upgradables.Find(x => x.IOType == type);
            if (upgradable == null) return 0;

            int nextLevel = item.Level + 1;
            if (nextLevel >= upgradable.UpgradebleObjectData.Count)
                return 0;

            return upgradable.UpgradebleObjectData[nextLevel].Price;
        }

        private void EmitUpgradeOffer(InteractiveObjectType type)
        {
            bool available = IsUpgradeAvailable(type);
            int price = available ? GetNextUpgradePrice(type) : 0;
            _localEvents.TriggerUpgradeOffer(type, available, price);
        }
        private Sprite GetSpriteFor(InteractiveObjectType type, int level, float dirt)
        {
            var upgradable = _upgradableConfig.Upgradables.Find(x => x.IOType == type);
            if (upgradable == null || upgradable.UpgradebleObjectData.Count == 0)
            {
                Debug.LogError($"[UpgradeLogic] No data for {type}");
                return null;
            }

            level = Mathf.Clamp(level, 0, upgradable.UpgradebleObjectData.Count - 1);
            var data = upgradable.UpgradebleObjectData[level];
            return dirt < DirtThreshold ? data.Clean : data.Dirty;
        }
        
        private static string BuildKey(InteractiveObjectType type)
            => $"upgradable:{type}";

        private void SaveItemState(InteractiveObjectType type, UpgradableItemState state)
        {
            string key = BuildKey(type);
            string json = JsonUtility.ToJson(state);
            _progressDataAdapter.SaveCustomJson(key, json);
        }

        private UpgradableItemState LoadItemState(InteractiveObjectType type)
        {
            string key = BuildKey(type);
            string json = _progressDataAdapter.LoadCustomJson(key);
            return string.IsNullOrEmpty(json)
                ? new UpgradableItemState { Level = 0, Dirt = 0f }
                : JsonUtility.FromJson<UpgradableItemState>(json);
        }

        public void CleanUp()
        {
            _localEvents.OnUpgradeItemByType  -= UpgradeItemByType;
            _localEvents.OnActiveSprintByType -= UpdateCleanLevel;
            _localEvents.OnIODirty            -= RefreshIODirty;
            _localEvents.OnPurchaseUpgradeResult -= OnPurchaseUpgradeResult;
        }
    }
}
