using Scripts.Meta;
using Scripts.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class HealthBarView : MonoBehaviour, IStatBarView
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TextMeshProUGUI _label;
        
        private MetaType _metaType = MetaType.Health;
        private float _initialWidth;
        private IStatBarView _iStatBarViewImplementation;
        public string DataKey => Consts.Food;

        private void Awake()
        {
            _initialWidth = _fillBar.rectTransform.rect.width;
        }
    
        public void UpdateView(float value, float maxValue) 
        {
            _label.text = Metatype.ToString();
        
            float newWidth = (value / maxValue) * _initialWidth;
        
            _fillBar.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                newWidth
            );
        }

        public MetaType Metatype => _metaType;
    }
}