using System.Collections.Generic;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class HealthStatPanel : MonoBehaviour
    {
        [SerializeField] private Transform _statContainer;
        [SerializeField] private StatInfo _statPrefab;
        
        private List<StatInfo> _stats = new();

        public void InitPanel(Dictionary<string, Metadata> stats, ProgressDataAdapterOLD progressDataAdapterOld)
        {
            for (int i = _statContainer.childCount - 1; i > 0; i--)
                Destroy(_statContainer.GetChild(i).gameObject);
            _stats.Clear();

            foreach (var kvp in stats)
            {
                var key = kvp.Key;                      
                var meta = kvp.Value;                   
                var item = Instantiate(_statPrefab, _statContainer);
                item.Init(key,  progressDataAdapterOld);
                item.UpdateInfo();                     
                _stats.Add(item);
            }
            
            Debug.Log("Stats count = " + _stats.Count);
        }
        
        public void UpdateStats()
        {
            foreach (var stat in _stats)
            {
                stat.UpdateInfo();
            }
        }
    }
}