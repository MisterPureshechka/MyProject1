using Core;
using Scripts.Progress;
using Scripts.Tasks;

namespace Scripts.Bugs
{
    public class BugLogic : ICleanUp
    {
        private ProgressDataAdapter _progressDataAdapter;
        
        private float _currentMood;
        private float _currentHunger;
        private float _currentEnergy;

        private void EmitBug(IDevTask devTask)
        {
            
        }
        public void CleanUp()
        {
            
        }
    }
}