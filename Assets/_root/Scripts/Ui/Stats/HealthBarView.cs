using _root.Scripts.Ui.Stats;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class HealthBarView : BarBase
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TextMeshProUGUI _label;
        
        private MetaType _metaType = MetaType.Health;
        private float _initialWidth;
        private IStatBarView _iStatBarViewImplementation;
        
        public override string DataKey => Consts.Food;
        
        private void Awake()
        {
            _initialWidth = _fillBar.rectTransform.rect.width;
        }
    
        public override void UpdateView(float value, float maxValue) 
        {
            _label.text = _metaType.ToString();
        
            float newWidth = (value / maxValue) * _initialWidth;
        
            _fillBar.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                newWidth
            );
        }

        public override MetaType MetaType => _metaType;
    }
}