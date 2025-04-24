using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class TaskSystemDemo : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameData _data;
        
        public Button enterCurrentSprintButton;
        public Button exitCurrentSprintButton;
        public Button addDevTaskButton;
        public Button addChillTaskButton;
        public Button restoreDevSprintTaskButton;
        
        public TextMeshProUGUI _allSprintsText;
        private SprintSystemTest _sprintSystem;

        private void Start()
        {
            var uiFactory = new UiFactory(_data);
            _sprintSystem = new SprintSystemTest(this, new TaskLibrary(), _canvas, _data, Object.FindAnyObjectByType<HUDView>().SprintView, uiFactory, new LocalEvents());
        }

        private void Update()
        {
            _sprintSystem.UpdateStats();
        }
        
    }
}