using System.Collections.Generic;
using Scripts.Tasks;

namespace Scripts.Tasks
{
    public sealed class MilestoneRunData
    {
        public int MilestoneIndex;
        public int MilestoneCount;

        public int DaysLimit;
        public int MoneyReward;

        public List<DevTask> Tasks;
    }
}