using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.EcoSystem.Calendar
{
    public class DayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform _eventHolder;
        [SerializeField] private TextMeshProUGUI _dayNumber;

        private Vector3 _defaultScale;
        private Vector3 _hoverScale = Vector3.one * 1.5f;

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _dayNumber.raycastTarget = false;
        }

        public void SetDayNumber(int dayNumber)
        {
            if (dayNumber == 0) _dayNumber.text = "";
            else _dayNumber.text = dayNumber.ToString();
        }

        public void AddEvent(DayEvent dayEvent)
        {
            dayEvent.transform.SetParent(_eventHolder, false);
            dayEvent.transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
            {
                dayEvent.transform.localScale = Vector3.one; 
            });
            
        }

        public void ClearEvents()
        {
            for (int i = 0; i < _eventHolder.childCount; i++)
            {
                Destroy(_eventHolder.GetChild(i).gameObject);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //_dayNumber.transform.DOScale(_hoverScale, 0.2f).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //_dayNumber.transform.DOScale(_defaultScale, 0.2f).SetEase(Ease.InBack);
        }
    }
}