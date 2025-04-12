using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Ui.TaskUi
{
    public class AllTaskView : MonoBehaviour
    {
        private List<TaskButtonsContainerView> _buttonsContainers = new List<TaskButtonsContainerView>();
        private Sequence _sequence;
        
        [SerializeField] private TaskPanelButtonView _taskButton;
        [SerializeField] private TaskButtonsContainerView _taskButtonsContainerView;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Transform _tasksContainer;

        public Action<ITask> OnTaskClicked;
        
        private Vector2 _startPosition;
        private Vector2 _hidePosition;
        private Vector2 offset = new Vector2(0, -400);

        private void Start()
        {
            _startPosition = _root.transform.position;
            _hidePosition = _startPosition + offset;
            HideAllTasksOnStart();
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
                    taskButton.UpdateInfo(task.Title);
                    taskButton.TaskPannelButton.onClick.AddListener(() => OnTaskClicked?.Invoke(task));
                }
                
                _buttonsContainers.Add(buttonsContainerView);
            }
        }

        private void HideAllTasks()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _root.gameObject.SetActive(true);
            _sequence.Append(_root.transform.DOMove(_hidePosition, 1.0f).SetEase(Ease.InSine));
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
            
            _root.gameObject.SetActive(true);
            _sequence.Append(_root.transform.DOMove(_startPosition, 1.0f).SetEase(Ease.OutSine));
            
        }
    }
}