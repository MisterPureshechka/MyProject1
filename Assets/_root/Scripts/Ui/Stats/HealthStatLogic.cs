using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class HealthStatLogic : ICleanUp
    {
        private HealthStatPanel _healthPanel;
        private ProgressDataAdapterOLD _progressDataAdapterOld;
        private LocalEvents _localEvents;

        public HealthStatLogic(HealthStatPanel healthPanel, ProgressDataAdapterOLD progressDataAdapterOld, LocalEvents localEvents)
        {
            _healthPanel = healthPanel;
            _progressDataAdapterOld = progressDataAdapterOld;
            _localEvents = localEvents;

            InitPanel();
            _progressDataAdapterOld.OnStatUpdated += UpdateStats;
        }

        private void InitPanel()
        {
            var healthStats = new Dictionary<string, Metadata>();
            // foreach (var kvp in _progressDataAdapter.GetProgressData().Metadata)
            // {
            //     if (kvp.Value.MetaType == MetaType.Health)
            //         healthStats[kvp.Key] = kvp.Value; 
            // }

            _healthPanel.InitPanel(healthStats, _progressDataAdapterOld);
        }

        private void UpdateStats()
        {
            _healthPanel.UpdateStats();
        }

        public void CleanUp()
        {
            Object.Destroy(_healthPanel.gameObject);
            _progressDataAdapterOld.OnStatUpdated -= UpdateStats;
        }
    }
}