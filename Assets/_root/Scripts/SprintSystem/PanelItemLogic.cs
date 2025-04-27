using System.Collections.Generic;

namespace Scripts.Tasks
{
    public class PanelItemLogic
    {
        private readonly Dictionary<SprintType, List<IPanelItem>> _panelItems = new();

        private void AddItemToPanel(IPanelItem panelItem, SprintType sprintType)
        {
            _panelItems[sprintType].Add(panelItem);
        }
        
    }
}