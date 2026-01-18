using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Ui;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Perks
{
    public class PerkService : IExecute, IPerkService, ICleanUp
    {
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;
        private readonly Dictionary<string, PerkData> _allPerks; 
        private readonly HashSet<string> _activePerks = new();

        private PerksCatalogue _perksCatalogue;
        
        private readonly Dictionary<string, float> _multipliers = new();  
        private readonly Dictionary<SprintType, int> _maxTasksAdd = new(); 
        
        private readonly ProgressDataAdapter _progressDataAdapter;    
        private readonly GameProgress _gameProgress;   

        public PerkService(UiFactory uiFactory, Canvas canvas, LocalEvents localEvents, ProgressDataAdapter progressDataAdapter, GameProgress gameProgress)
        {
            _gameProgress = gameProgress;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _allPerks = PerkLoader.Load();
            _perksCatalogue = _uiFactory.GetPerksCatalogue(canvas.transform);
            _perksCatalogue.OnApplySelectedPerks += ApplyCatalogueSelection;
            
            LoadPerks();

            _localEvents.OnHeroWokeUp += ShowCatalogue;
        }

        private void LoadPerks()
        {
            var loadedIds = _progressDataAdapter.GetActivePerkIds();
            foreach (var id in loadedIds)
            {
                if (_allPerks.ContainsKey(id))
                    _activePerks.Add(id);
                Debug.Log($"Loaded perk {id}");
            }
        }

        public  void ShowCatalogue()
        {
            Debug.Log("Showing catalogue");
            if (_perksCatalogue == null)
            {
                Debug.LogError("PerksCatalogue is null");
                return;
            }

            _perksCatalogue.GetPerks(_allPerks, _activePerks);
            _localEvents.TriggerShowCatalogue(_perksCatalogue);
        }

        public void HideCatalogue()
        {
            _localEvents.TriggerHideCatalogue(_perksCatalogue);
        }

        public float GetEffectMultiplier(string effectKey)
        {
            return _multipliers.TryGetValue(effectKey, out var m) ? m : 1f;
        }

        public int ModifyMaxActiveTasks(SprintType sprint, int baseValue)
        {
            return baseValue + _maxTasksAdd.GetValueOrDefault(sprint, 0);
        }

        public float ModifyInterval(SprintType sprint, float baseInterval)
        {
            float multiplier = GetEffectMultiplier($"{sprint}.Interval");
            return baseInterval * multiplier;
        }

        public void OnSprintStart(SprintType sprint) { }
        public void OnSprintEnd(SprintType sprint) { }

        private void ApplyCatalogueSelection(List<string> selectedIds)
        {
            if (selectedIds == null) return;

            HideCatalogue();

            var selectedSet = new HashSet<string>(selectedIds);
            var toRemove = new List<string>();
            foreach (var id in _activePerks)
                if (!selectedSet.Contains(id))
                    toRemove.Add(id);

            foreach (var id in toRemove)
                _activePerks.Remove(id);

            foreach (var id in selectedSet)
                if (_allPerks.ContainsKey(id))
                    _activePerks.Add(id);
            
            Debug.Log($"Selected perks: {string.Join(", ", selectedSet)}");

            PersistAndRebuildCache();
        }

        private void PersistAndRebuildCache()
        {
            _multipliers.Clear();
            _maxTasksAdd.Clear();

            void Multiply(string key, float value)
            {
                if (!_multipliers.ContainsKey(key)) _multipliers[key] = 1f;
                _multipliers[key] *= value;
            }

            void AddMaxTasks(string sprintName, float add)
            {
                if (!Enum.TryParse<SprintType>(sprintName, out var sprint)) return;
                if (!_maxTasksAdd.ContainsKey(sprint)) _maxTasksAdd[sprint] = 0;
                _maxTasksAdd[sprint] += Mathf.RoundToInt(add);
            }

            foreach (var perkId in _activePerks)
            {
                if (!_allPerks.TryGetValue(perkId, out var def) || def?.Effects == null)
                    continue;

                foreach (var kv in def.Effects)
                {
                    string key = kv.Key;
                    float val  = kv.Value;
                    if (string.IsNullOrEmpty(key)) continue;

                    if (key.EndsWith(".MaxActiveTasks", StringComparison.Ordinal))
                    {
                        var sprintName = key.Substring(0, key.IndexOf(".MaxActiveTasks", StringComparison.Ordinal));
                        AddMaxTasks(sprintName, val);
                    }
                    else
                    {
                        // ReadKnowledgeEffect, <Sprint>.<Stat>, <Sprint>.Interval, TaskInterval.*
                        Multiply(key, val);
                    }
                }
            }
            
            _progressDataAdapter.SetActivePerkIds(_activePerks);
            _gameProgress.SaveProgress(_progressDataAdapter.GetProgressData());
        }

        public void CleanUp()
        {
            _perksCatalogue.OnApplySelectedPerks -= ApplyCatalogueSelection;
            if (_perksCatalogue != null)
            {
                Object.Destroy(_perksCatalogue.gameObject);
            }
        }

        public void Execute(float deltatime)
        {
            if (Input.GetKeyDown(KeyCode.A)) ShowCatalogue();
        }
    }
}
