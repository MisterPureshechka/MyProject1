using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class ReadTaskCatalogue : MonoBehaviour
    {
        [SerializeField] private Transform _tasksContainer;
        [SerializeField] private ReadTaskButton _readTaskButton;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        
        public Action<IReadTask> OnTaskClicked;
        public Action OnCloseButtonClicked;
        public Action OnApplyButtonClicked;
        
        private Sequence _sequence;
        
        private Vector2 _startPosition;
        private Vector2 _hidePosition;
        private Vector2 _offset = new Vector2(0, -1000);
        private LocalEvents _localEvents;

        private void Start()
        {
            _startPosition = _tasksContainer.transform.position;
            _hidePosition = _startPosition + _offset;

            HideAllTasksOnStart();

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
            HideAllTasks();
        }
        
        private void AddReadTask(IReadTask readTask)
        {
            OnTaskClicked?.Invoke(readTask);
            ShowApplyButton();
        }
        
        private void ApplyButtonClickListener()
        {
            OnApplyButtonClicked?.Invoke();
            HideAllTasks();
        }
        
        private void ShowApplyButton()
        {
            _applyButton.gameObject.SetActive(true);
            _applyButton.onClick.AddListener(ApplyButtonClickListener);
        }
        
        public void ShowCatalogue()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            Debug.LogError("Task shown insight");
            
            gameObject.SetActive(true);
            _sequence.Append(gameObject.transform.DOMove(_startPosition, 0.6f).SetEase(Ease.OutSine));
            
        }
        
        public void HideAllTasks()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            gameObject.SetActive(true);
            _sequence.Append(gameObject.transform.DOMove(_hidePosition, 0.4f).SetEase(Ease.InSine));
        }
        
        private void HideAllTasksOnStart()
        {
            gameObject.gameObject.SetActive(false);
            gameObject.transform.position = _hidePosition;
        }
        
        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }
        
        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
            _applyButton.onClick.RemoveAllListeners();
        }
    }
}