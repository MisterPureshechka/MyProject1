using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Perks;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Scripts.Stat
{
    public class StatEffectLogic : ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;
        private readonly Dictionary<string, Dictionary<string, float>> _effects;
        private IPerkService _perkService;
        private DevTaskType _activeReadTaskType;
        

        public StatEffectLogic(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents, IPerkService perkService)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            _perkService = perkService;

            _effects = StatEffectLoader.Load(); 
            _localEvents.OnActiveSprintByType += OnSprintActivated;
            _localEvents.OnReadTaskUpdate += ReadTaskUpdateListener; 
        }
        
        private void ReadTaskUpdateListener(DevTaskType readTask)
        {
            _activeReadTaskType = readTask;
        }

        private void OnSprintActivated(SprintType sprintType)
        {
            if (sprintType == SprintType.Read)
            {
                string knowledgeKey = _activeReadTaskType.ToString(); 
    
                if (_effects.TryGetValue("ReadKnowledgeEffect", out var effectDict) &&
                    effectDict.TryGetValue(knowledgeKey, out float baseDelta))
                {
                    float mult = _perkService.GetEffectMultiplier("ReadKnowledgeEffect");
                    _progressDataAdapter.TryUpdateValue(knowledgeKey, baseDelta * mult);
                }
                else
                {
                    Debug.LogWarning($"No effect found for key: {knowledgeKey} in Read sprint");
                }

            }
            
            string actionKey = sprintType.ToString();

            if (!_effects.TryGetValue(actionKey, out var statChanges))
            {
                Debug.LogWarning($"No stat effects found for action: {actionKey}");
                return;
            }

            foreach (var pair in statChanges)
            {
                string statKey = pair.Key;
                float baseDelta = pair.Value;
                float mult = _perkService.GetEffectMultiplier($"{actionKey}.{statKey}");
                _progressDataAdapter.TryUpdateValue(statKey, baseDelta * mult);
            }
        }

        public void CleanUp()
        {
            _localEvents.OnActiveSprintByType -= OnSprintActivated;
        }
    }
}