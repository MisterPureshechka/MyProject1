using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _root.Scripts.Ui.Stats
{
    public abstract class BarBase : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IStatBarView
    {
        private LocalEvents _localEvents;
        
        public abstract string DataKey { get; }
        public abstract void UpdateView(float value, float maxValue);

        public abstract MetaType MetaType { get; }

        public virtual void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _localEvents.TriggerMouseEnterStat(MetaType);
            _localEvents.TriggerMouseMoveStat(eventData.position);
            Debug.Log($"Вошел в: {MetaType}");
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _localEvents.TriggerMouseMoveStat(eventData.position);
            Debug.Log($"Курсор поверх: {MetaType}");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _localEvents.TriggerMouseExitStat(); 
            Debug.Log($"Вышел из: {MetaType}");
        }
    }
}