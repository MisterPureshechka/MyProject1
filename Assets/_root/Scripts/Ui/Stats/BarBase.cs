using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _root.Scripts.Ui.Stats
{
    public abstract class BarBase : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler, IStatBarView
    {
        private LocalEvents _localEvents;
        
        public abstract string DataKey { get; }
        public abstract void UpdateView(float value, float maxValue);

        public abstract MetaType MetaType { get; }

        public virtual void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }
        
        public void OnPointerMove(PointerEventData eventData)
        {
            var pointer = eventData.position;
            _localEvents.TriggerMouseOverStat(MetaType, pointer); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _localEvents.TriggerMouseExitStat(); 
        }
    }
}