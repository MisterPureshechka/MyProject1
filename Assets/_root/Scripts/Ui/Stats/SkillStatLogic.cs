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
        private ProgressDataAdapterOLD _progressDataAdapterOld;
        private LocalEvents _localEvents;

        public SkillStatLogic(SkillStatPanel skillPanel, ProgressDataAdapterOLD progressDataAdapterOld, LocalEvents localEvents)
        {
            _skillPanel = skillPanel;
            _progressDataAdapterOld = progressDataAdapterOld;
            _localEvents = localEvents;
            
            InitPanel();
            _progressDataAdapterOld.OnStatUpdated += UpdateStats;
        }
        
        private void InitPanel()
        {
        }

        private void UpdateStats()
        {
            _skillPanel.UpdateStats();
        }

        public void CleanUp()
        {
            _progressDataAdapterOld.OnStatUpdated -= UpdateStats;
        }
    }
}