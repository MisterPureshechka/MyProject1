using Scripts.Progress;
using TMPro;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class StatInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private StatProgressBar _progressBar;
        
        private string _key; 
        private ProgressDataAdapter _adapter;
        private string _tooltip;

        public void Init(string metaKey, ProgressDataAdapter adapter)
        {
            _key = metaKey;
            _adapter = adapter;
            _title.text = metaKey;
        }

        public void UpdateInfo()
        {
            var meta = _adapter.GetMetadata(_key);
            if (meta == null) return;
            
            float value = meta.Value;
            float max = Mathf.Max(1f, meta.MaxValue);
            float norm = Mathf.Clamp01(value / max);
            
            _valueText.text = Mathf.RoundToInt(norm * 100f) + "%";
            _progressBar.UpdateProgressBar(norm);
        }
    }
}