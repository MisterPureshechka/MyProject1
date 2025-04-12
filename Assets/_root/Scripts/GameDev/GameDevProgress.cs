using System;
using System.Collections.Generic;
using Scripts.Progress;
using Scripts.Tasks;

namespace Scripts.GameDev
{
    public class GameDevProgress
    {
        private ProgressDataAdapter _progressDataAdapter;
        
        private Dictionary<string, int> _completedTaskTitleCounter = new();
        private Dictionary<DevTaskType, int> _completedTaskTypeCounter = new();

        public GameDevProgress()
        {
            GetOrCreateDevTypeCounter();
        }

        private void GetOrCreateDevTypeCounter()
        {
            foreach (DevTaskType type in Enum.GetValues(typeof(DevTaskType)))
            {
                _completedTaskTypeCounter[type] = 0;
            }
        }

        public void CompleteTaskByTitle(IDevTask task)
        {
            var title = task.Title;
            
            if(_completedTaskTitleCounter.ContainsKey(title))
            {
                _completedTaskTitleCounter[title]++;
            }
            else
            {
                _completedTaskTitleCounter.Add(title, 1);
            }

            _completedTaskTypeCounter[task.Type]++;
        }
    }
}