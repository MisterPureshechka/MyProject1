using Core;
using Scripts.Progress;
using Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Scripts.Ui
{
    public class StatsDebuger : MonoBehaviour, IExecute
    {
        [field: SerializeField] private TextMeshProUGUI AllStats;
        
        private ProgressData _progressData;

        public void Init(ProgressData progressData)
        {
            _progressData = progressData;
        }

        private void UpdateStats()
        {
            string result = "";
            foreach (var key in _progressData.Metadata.Keys)
            {
                result += $"{key}: {_progressData.Metadata[key].Value}\n";
            }
            
            AllStats.text = result;
        }

        public void Execute(float deltatime)
        {
            UpdateStats();
        }
    }
}