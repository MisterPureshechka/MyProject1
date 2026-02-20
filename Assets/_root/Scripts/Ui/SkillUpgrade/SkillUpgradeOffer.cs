using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Ui.SkillUpgrade
{
    public class SkillUpgradeOffer
    {
        public string Id;
        public Dictionary<DevTaskType, float> SkillUpgradeMap;
        public int SkillUpgradeCost;
    }
}