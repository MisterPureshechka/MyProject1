using Scripts.Meta;
using Scripts.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class KnowledgeBarView : MonoBehaviour, IStatBarView
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TextMeshProUGUI _label;
        
        private MetaType _metaType = MetaType.Knowledge;
        
        private float _initialWidth; 
        public string DataKey => Consts.Programming;

        public MetaType Metatype => _metaType;

        private void Awake()
        {
            _initialWidth = _fillBar.rectTransform.rect.width;
        }
    
        public void UpdateView(float value, float maxValue) 
        {
            _label.text = _metaType.ToString();
        
            float newWidth = (value / maxValue) * _initialWidth;
        
            _fillBar.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                newWidth
            );
        }
    }
}