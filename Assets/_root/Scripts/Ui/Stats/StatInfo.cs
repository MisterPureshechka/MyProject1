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
        private ProgressDataAdapterOLD _adapterOld;
        private string _tooltip;

        public void Init(string metaKey, ProgressDataAdapterOLD adapterOld)
        {
            _key = metaKey;
            _adapterOld = adapterOld;
            _title.text = metaKey;
        }

        public void UpdateInfo()
        {
            var meta = _adapterOld.GetMetadata(_key);
            if (meta == null) return;
            
            float value = meta.Value;
            float max = Mathf.Max(1f, meta.MaxValue);
            float norm = Mathf.Clamp01(value / max);
            
            _valueText.text = Mathf.RoundToInt(norm * 100f) + "%";
            _progressBar.UpdateProgressBar(norm);
        }
        
        public void UpdateInfo(float value, float maxValue)
        {
            float max = Mathf.Max(1f, maxValue);
            float norm = Mathf.Clamp01(value / max);
            
            _valueText.text = Mathf.RoundToInt(norm * 100f) + "%";
            _progressBar.UpdateProgressBar(norm);
        }
    }
}