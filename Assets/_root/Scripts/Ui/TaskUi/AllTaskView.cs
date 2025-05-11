using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class AllTaskView : MonoBehaviour
    {
        private List<TaskButtonsContainerView> _buttonsContainers = new List<TaskButtonsContainerView>();
        private Sequence _sequence;
        private LocalEvents _localEvents;
        
        [SerializeField] private TaskPanelButtonView _taskButton;
        [SerializeField] private TaskButtonsContainerView _taskButtonsContainerView;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Transform _tasksContainer;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _newRoot;

        public Action<ITask> OnTaskClicked;
        public Action OnCloseButtonClicked;
        public Action OnApplyButtonClicked;
        
        private Vector2 _startPosition;
        private Vector2 _hidePosition;
        private Vector2 _offset = new Vector2(0, -1000);
        
        private void Start()
        {
            _startPosition = _root.transform.position;
            _hidePosition = _startPosition + _offset;
            _applyButton.gameObject.SetActive(false);
            HideAllTasksOnStart();
        }

        public void Init(LocalEvents events)
        {
            _localEvents = events;
        }

        public void SetDevTasks(Dictionary<DevTaskType, List<IDevTask>> devTasks)
        {
            foreach (var taskType in devTasks.Keys)
            { 
                var buttonsContainerView = Instantiate(_taskButtonsContainerView, _tasksContainer).GetComponentInChildren<TaskButtonsContainerView>();
                buttonsContainerView.UpdateText(taskType.ToString());
                
                foreach (var task in devTasks[taskType])
                {
                    var taskButton = Instantiate(_taskButton, buttonsContainerView.transform).GetComponent<TaskPanelButtonView>();
                    taskButton.UpdateInfo(task.Title, task.Type);
                    buttonsContainerView.AddToRoot(taskButton.gameObject);
                    taskButton.TaskPannelButton.onClick.AddListener(() => AddDevTask(task));
                }

                _buttonsContainers.Add(buttonsContainerView);
            }

            _closeButton.onClick.AddListener(CloseButtonClickListener);
        }

        private void CloseButtonClickListener()
        {
            OnCloseButtonClicked?.Invoke();
            HideAllTasks();
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

        private void AddDevTask(IDevTask devTask)
        {
            OnTaskClicked?.Invoke(devTask);
            ShowApplyButton();
        }

        private void HideAllTasks()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _root.gameObject.SetActive(true);
            _sequence.Append(_root.transform.DOMove(_hidePosition, 0.4f).SetEase(Ease.InSine));
        }

        private void HideAllTasksOnStart()
        {
            _root.gameObject.SetActive(false);
            _root.position = _hidePosition;
        }


        public void ShowAllTasks()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _applyButton.gameObject.SetActive(false);
            
            Debug.LogError("Task shown insight");
            
            foreach (var containers in _buttonsContainers)
            {
                containers.UpdateAllItemsPositions();
            }
            
            _root.gameObject.SetActive(true);
            _sequence.Append(_root.transform.DOMove(_startPosition, 0.6f).SetEase(Ease.OutSine));
            
        }
        
        
    }
}