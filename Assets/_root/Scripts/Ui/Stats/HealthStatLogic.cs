using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;

namespace _root.Scripts.Ui.Stats
{
    public class HealthStatLogic : ICleanUp
    {
        private HealthStatPanel _healthPanel;
        private ProgressDataAdapter _progressDataAdapter;
        private LocalEvents _localEvents;

        public HealthStatLogic(HealthStatPanel healthPanel, ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _healthPanel = healthPanel;
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;

            InitPanel();
            _progressDataAdapter.OnStatUpdated += UpdateStats;
        }

        private void InitPanel()
        {
            var healthStats = new Dictionary<string, Metadata>();
            foreach (var kvp in _progressDataAdapter.GetProgressData().Metadata)
            {
                if (kvp.Value.MetaType == MetaType.Health)
                    healthStats[kvp.Key] = kvp.Value; 
            }

            _healthPanel.InitPanel(healthStats, _progressDataAdapter);
        }

        private void UpdateStats()
        {
            _healthPanel.UpdateStats();
        }

        public void CleanUp()
        {
            _progressDataAdapter.OnStatUpdated -= UpdateStats;
        }
    }
}