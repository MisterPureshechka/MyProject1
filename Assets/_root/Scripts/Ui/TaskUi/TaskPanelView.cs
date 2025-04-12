using System;
using System.Collections.Generic;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Scripts.Ui.TaskUi
{
    public class TaskPanelView : MonoBehaviour
    {
        private RectTransform _panelRectTransform;
        
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _buttonsRoot;
        
        public Action<ITask> OnTaskClicked;
        
        //надо закинуть в ДАТУ!!!
        [SerializeField] private GameObject _taskButtonPrefab;
        
        private List<GameObject> _currentButtons = new List<GameObject>();
        private TaskLibrary _taskLibrary;
        private LocalEvents _localEvents;

        private void Awake()
        {
            _panelRectTransform = GetComponent<RectTransform>();
        }
        
        public void ShowTasksForSprint(SprintType sprintType, Vector3 screenPosition, Action<bool> isSupportedTypeOnComplete = null)
        {
            ClearCurrentButtons();
            gameObject.SetActive(true);
            SetPositionOnCanvas(screenPosition);
            switch (sprintType)
            {
                case SprintType.Dev:
                    AddTasksToPanel(_taskLibrary.GetTasks<IDevTask>());
                    isSupportedTypeOnComplete?.Invoke(true);
                    break;
                case SprintType.Eat:
                    AddTasksToPanel(_taskLibrary.GetTasks<EatTask>());
                    isSupportedTypeOnComplete?.Invoke(true);
                    break;
                default:
                    isSupportedTypeOnComplete?.Invoke(false);
                    Debug.LogWarning($"Unsupported sprint type: {sprintType}");
                    break;
            }
            
        }

        public void ShowCurrentCommand(SprintType sprintType)
        {
            
        }

        public void SetPositionOnCanvas(Vector3 normilizedPosition)
        {
            Vector3 screenPosition = new Vector3(
                (normilizedPosition.x + 1f) * 0.5f * Screen.width,
                (normilizedPosition.y + 1.5f) * 0.5f * Screen.height,
                0
            );
    
            _panelRectTransform.position = screenPosition;
        }

        private void AddTasksToPanel<T>(List<T> tasks) where T : ITask
        {
            foreach (var task in tasks)
            {
                var button = Instantiate(_taskButtonPrefab, _buttonsRoot.transform);
                var buttonView = button.GetComponent<TaskPanelButtonView>();
                
                buttonView.UpdateInfo(task.Title);
                buttonView.TaskPannelButton.onClick.AddListener(() => OnTaskClicked?.Invoke(task));
                _applyButton.onClick.AddListener(ApplyTaskListener);
                
                _currentButtons.Add(button);
            }
        }

        private void ShowPanelItems<T>(List<T> panelItem) where T : IPanelItem
        {
            
        }

        private void ApplyTaskListener()
        {
            _localEvents.TriggerTasksApply();
            HidePanel();
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }

        private void ClearCurrentButtons()
        {
            foreach (var button in _currentButtons)
            {
                Destroy(button);
            }
            _currentButtons.Clear();
        }

        public void Init(TaskLibrary library, LocalEvents localEvents)
        {
            _taskLibrary = library;
            _localEvents = localEvents;
        }
    }
}