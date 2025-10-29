using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Upgrade;
using UnityEngine;

namespace Scripts.Upgrade
{
    public class UpgradeLogicOLD : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private ChairView _chairView;
        private WorkspaceView _workspaceView;
        
        private readonly Dictionary<int, float> _chairEnergyEffects = new()
        {
            { 0, -0.0005f }, 
            { 1, -0.0003f }, 
            { 2, -0.0001f }, 
            { 3,  0f }       
        };

        public UpgradeLogicOLD(LocalEvents localEvents, ProgressDataAdapter progressDataAdapter)
        {
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;

            _chairView = Object.FindObjectOfType<ChairView>();
            _localEvents.OnUpgradeItem += UpgradeItem;
        }

        public void UpgradeItem(int id, UpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case UpgradeType.Chair:
                    
                    _chairView.Upgrade(id);

                    if (_chairEnergyEffects.TryGetValue(id, out float energyEffect))
                    {
                        var effects = StatEffectLoader.Load();

                        if (effects.TryGetValue("Dev", out var devEffects))
                        {
                            devEffects["Energy"] = energyEffect;
                        }
                    }
                    break;
                case UpgradeType.Workspace:
                    _workspaceView.Upgrade(id);
                    break;
            }
        }

        public void CleanUp()
        {
            _localEvents.OnUpgradeItem -= UpgradeItem;
        }
    }

    public enum UpgradeType
    {
        Chair,
        Workspace
    }
}