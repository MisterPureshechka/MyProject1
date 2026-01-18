using System.Collections.Generic;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;
using UnityEngine.Serialization;

namespace _root.Scripts.Ui.Stats
{
    public class SkillStatPanel : MonoBehaviour
    {
        [SerializeField] private Transform _skillContainer;
        [FormerlySerializedAs("_skillInfoPrefab")] [SerializeField] private SkillInfoOld skillInfoOldPrefab;
        
        private List<SkillInfoOld> _skills = new();
        
        public void InitPanel(Dictionary<string, Metadata> stats, ProgressDataAdapter progressDataAdapter)
        {
            for (int i = _skillContainer.childCount - 1; i > 0; i--)
                Destroy(_skillContainer.GetChild(i).gameObject);
            _skills.Clear();

            foreach (var kvp in stats)
            {
                var key = kvp.Key;                      
                var meta = kvp.Value;                   
                var item = Instantiate(skillInfoOldPrefab, _skillContainer);
                item.Init(key,  progressDataAdapter);
                item.UpdateInfo();                     
                _skills.Add(item);
            }
            
            Debug.Log("Stats count = " + _skills.Count);
        }
        
        public void UpdateStats()
        {
            foreach (var stat in _skills)
            {
                stat.UpdateInfo();
            }
        }
    }
}