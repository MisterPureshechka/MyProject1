using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
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
        private DevTaskType _activeReadTaskType;

        public StatEffectLogic(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;

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
                    effectDict.TryGetValue(knowledgeKey, out float delta))
                {
                    _progressDataAdapter.TryUpdateValue(knowledgeKey, delta);
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
                float delta = pair.Value;

                _progressDataAdapter.TryUpdateValue(statKey, delta);
            }
        }

        public void CleanUp()
        {
            _localEvents.OnActiveSprintByType -= OnSprintActivated;
        }
    }
}