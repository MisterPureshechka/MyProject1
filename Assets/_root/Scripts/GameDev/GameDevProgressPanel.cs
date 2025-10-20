using System;
using System.Collections.Generic;
using Scripts.Progress;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.GameDev
{
    public class GameDevProgressPanel : MonoBehaviour
    {
        [SerializeField] private Transform _progressContainer;
        [SerializeField] private ProgressInfo _progressInfo;

        private readonly List<ProgressInfo> _items = new();
        private GameDevProgress _devProgress;
        private TaskLibrary _taskLibrary; 
        private string _gameName;

        private static readonly DevTaskType[] Order =
        {
            DevTaskType.Programming,
            DevTaskType.Art,
            DevTaskType.GameDesign,
            DevTaskType.Marketing,
            DevTaskType.SoundDesign
        };
        public void Init(GameDevProgress devProgress, string gameName, TaskLibrary taskLibrary = null)
        {
            _devProgress = devProgress;
            _gameName = gameName;
            _taskLibrary = taskLibrary;

            Rebuild();
        }

        public void Rebuild()
        {
            for (int i = _progressContainer.childCount - 1; i > 0; i--)
                Destroy(_progressContainer.GetChild(i).gameObject);
            _items.Clear();

            Dictionary<DevTaskType, int> totals = null;
            if (_taskLibrary != null)
            {
                totals = new Dictionary<DevTaskType, int>();
                var all = _taskLibrary.GetAlLDevTasks(); 
                foreach (DevTaskType t in Enum.GetValues(typeof(DevTaskType)))
                    totals[t] = all.TryGetValue(t, out var list) ? list.Count : 0;
            }

            var data = _devProgress.GetGameProgress(_gameName);
            foreach (var type in Order)
            {
                int completed = (data != null && data.CompletedByType.TryGetValue(type, out var c)) ? c : 0;
                int total = totals != null ? totals[type] : -1;

                var item = Instantiate(_progressInfo, _progressContainer);
                item.InitTitle(type.ToString());
                item.SetValue(completed);
                _items.Add(item);
            }
        }

        public void RefreshValues(DevTaskType taskType)
        {
            int idx = Array.IndexOf(Order, taskType);
            if (idx < 0 || idx >= _items.Count) return;

            var data = _devProgress.GetGameProgress(_gameName);
            int completed = (data != null && data.CompletedByType.TryGetValue(taskType, out var c)) ? c : 0;

            _items[idx].SetValue(completed);
            _items[idx].AniamteText();
        }
    }
}
