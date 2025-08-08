using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Catalogues;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class ReadTaskCatalogue : MonoBehaviour, ICatalogue
    {
        [SerializeField] private Transform _tasksContainer;
        [SerializeField] private RectTransform _raedTasksCatalogue;
        [SerializeField] private ReadTaskButton _readTaskButton;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        
        public Action<IReadTask> OnTaskClicked;
        public Action OnApplyButtonClicked { get; set; }
        public Action OnCloseButtonClicked { get; set; }
        
        private Sequence _sequence;
        
        private Vector2 _startPosition = new Vector2(0,-75);
        private Vector2 _hidePosition;
        private Vector2 _offset = new Vector2(0, -400);
        private LocalEvents _localEvents;

        private void Start()
        {
            _hidePosition = _startPosition + _offset;

            HideAllTasksOnStart();
        }

        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }
        
        public void SetReadTask(List<IReadTask> readTasks)
        {
            foreach (var readTask in readTasks)
            {
                var taskButton = Instantiate(_readTaskButton, _tasksContainer).GetComponent<ReadTaskButton>();
                taskButton.SetInfo(readTask.Title);
                taskButton.Button.onClick.AddListener(() => AddReadTask(readTask));
            }
            
            _closeButton.onClick.AddListener(CloseButtonClickListener);
        }

        private void CloseButtonClickListener()
        {
            OnCloseButtonClicked?.Invoke();
            _localEvents.TriggerHideCatalogue(this);
        }
        
        private void AddReadTask(IReadTask readTask)
        {
            OnTaskClicked?.Invoke(readTask);
            ShowApplyButton();
        }
        
        private void ApplyButtonClickListener()
        {
            OnApplyButtonClicked?.Invoke();
            _localEvents.TriggerHideCatalogue(this);
        }
        
        private void ShowApplyButton()
        {
            _applyButton.gameObject.SetActive(true);
            _applyButton.onClick.AddListener(ApplyButtonClickListener);
        }
        
        public void Show(Action onComplete)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            gameObject.SetActive(true);
            _sequence.Append(_raedTasksCatalogue.DOLocalMove(_startPosition, 0.6f).SetEase(Ease.OutSine).OnComplete(
                () =>
                {
                    onComplete?.Invoke();
                }));
            
        }
        
        public void Hide(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            gameObject.SetActive(true);
            _sequence.Append(_raedTasksCatalogue.DOLocalMove(_hidePosition, 0.4f).SetEase(Ease.InSine).OnComplete(
                () =>
                {
                    onComplete?.Invoke();
                }));
        }

        public bool IsVisible { get; }

        private void HideAllTasksOnStart()
        {
            gameObject.SetActive(false);
            _raedTasksCatalogue.localPosition = _hidePosition;
        }
        
        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
            _applyButton.onClick.RemoveAllListeners();
        }
    }
}