using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;

namespace _root.Scripts.Ui.Stats
{
    public class SkillStatLogic : ICleanUp
    {
        private SkillStatPanel _skillPanel;
        private ProgressDataAdapter _progressDataAdapter;
        private LocalEvents _localEvents;

        public SkillStatLogic(SkillStatPanel skillPanel, ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _skillPanel = skillPanel;
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            
            InitPanel();
            _progressDataAdapter.OnStatUpdated += UpdateStats;
        }
        
        private void InitPanel()
        {
            var skillStat = new Dictionary<string, Metadata>();
            foreach (var kvp in _progressDataAdapter.GetProgressData().Metadata)
            {
                if (kvp.Value.MetaType == MetaType.Knowledge)
                    skillStat[kvp.Key] = kvp.Value; 
            }

            _skillPanel.InitPanel(skillStat, _progressDataAdapter);
        }

        private void UpdateStats()
        {
            _skillPanel.UpdateStats();
        }

        public void CleanUp()
        {
            _progressDataAdapter.OnStatUpdated -= UpdateStats;
        }
    }
}