using Scripts.Progress;
using Scripts.Ui;
using TMPro;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class SkillInfoOld : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _skillName;
        [SerializeField] private TextMeshProUGUI _skillValue;
        [SerializeField] private BaseTextAnimation _textAnimation;
        private string _key;
        private ProgressDataAdapterOLD _adapterOld;

        public void UpdateInfo()
        {
            var meta = _adapterOld.GetMetadata(_key);
            if (meta == null) return;
            
            float value = meta.Value;
            
            if (Mathf.Approximately(value % 1f, 0f))
                _skillValue.text = ((int)value).ToString();
            else
                _skillValue.text = value.ToString("F1");
        }

        public void Init(string metaKey, ProgressDataAdapterOLD adapterOld)
        {
            _key = metaKey;
            _adapterOld = adapterOld;
            _skillName.text = metaKey;
        }
    }
}
namespace _root.Scripts.Ui.Stats
{
}
