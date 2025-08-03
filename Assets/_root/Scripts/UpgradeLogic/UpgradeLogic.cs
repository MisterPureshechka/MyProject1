using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.UpgradeLogic
{
    public class UpgradeLogic : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private ChairView _chairView;
        private WorkspaceView _workspaceView;
        
        private readonly Dictionary<int, float> _chairEnergyEffects = new()
        {
            { 0, -0.0005f }, // старый стул
            { 1, -0.0003f }, // средний
            { 2, -0.0001f }, // крутой стул
            { 3,  0f }       // топовый — вообще не тратит энергию
        };

        public UpgradeLogic(LocalEvents localEvents, ProgressDataAdapter progressDataAdapter)
        {
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;

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
                        
                        Debug.Log($"Chair upgraded to ID {id}, new Dev Energy effect: {energyEffect}");
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