using System;
using System.Collections.Generic;
using Scripts.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Scripts.Ui.TaskUi
{
    public class TaskPanelView : MonoBehaviour
    {
        public Action<ITask> OnTaskClicked;
        
        //надо закинуть в ДАТУ!!!
        [SerializeField] private GameObject _taskButtonPrefab;
        
        private List<GameObject> _currentButtons = new List<GameObject>();
        private TaskLibrary _taskLibrary;

        public void ShowTasksForSprint(SprintType sprintType)
        {
            gameObject.SetActive(true);
            ClearCurrentButtons();
            
            switch (sprintType)
            {
                case SprintType.Dev:
                    AddTasksToPanel(_taskLibrary.GetDevTasks());
                    break;
                case SprintType.Eat:
                    AddTasksToPanel(_taskLibrary.GetEatTasks());
                    break;
                default:
                    Debug.LogWarning($"Unsupported sprint type: {sprintType}");
                    break;
            }
            
        }

        private void AddTasksToPanel<T>(List<T> tasks) where T : ITask
        {
            foreach (var task in tasks)
            {
                var button = Instantiate(_taskButtonPrefab, gameObject.transform);
                var buttonView = button.GetComponent<TaskPanelButtonView>();
                
                buttonView.UpdateInfo(task.Title);
                buttonView.TaskPannelButton.onClick.AddListener(() => OnTaskClicked?.Invoke(task));
                
                _currentButtons.Add(button);
            }
        }

        private void HidePanel()
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

        public void SetTaskLibrary(TaskLibrary library)
        {
            _taskLibrary = library;
        }
    }
}