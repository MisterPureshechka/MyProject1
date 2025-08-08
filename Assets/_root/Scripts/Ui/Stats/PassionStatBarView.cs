using _root.Scripts.Ui.Stats;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class PassionStatBarView : BarBase, IStatBarView
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TextMeshProUGUI _label;
        
        private MetaType _metaType = MetaType.Passion;
        
        private float _initialWidth;
        private StatsController _statsController;

        public override string DataKey => Consts.Programming;

        private void Awake()
        {
            _initialWidth = _fillBar.rectTransform.rect.width;
        }
    
        public override void UpdateView(float value, float maxValue) 
        {
            _fillBar.raycastTarget = !(value <= 0);
            
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