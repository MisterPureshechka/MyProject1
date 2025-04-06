using Scripts.Meta;
using Scripts.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class PassionStatBarView : MonoBehaviour, IStatBarView
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TextMeshProUGUI _label;
        
        public MetaType _metaType = MetaType.Passion;
        private float _initialWidth; 
        public string DataKey => Consts.Energy;

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