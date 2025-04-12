using Core;
using Scripts.GlobalStateMachine;
using Scripts.Ui;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CommandSystem : ICleanUp
    {
        private readonly Camera _camera;
        private readonly UiFactory _uiFactory;
        private readonly LocalEvents _localEvents;

        private CommandPanelView _commandPanelView;
        
        private CommandManager _commandManager;

        public CommandSystem(Canvas canvas, Camera camera, UiFactory uiFactory, LocalEvents localEvents)
        {
            _camera = camera;
            _uiFactory = uiFactory;
            _localEvents = localEvents;
            _commandPanelView = _uiFactory.GetCommandPanel(canvas.transform);
            _commandPanelView.gameObject.SetActive(false);
            
            _commandManager = new CommandManager();

            _localEvents.OnMouseClickIO += ShowCommandsByType;
            _localEvents.OnClickEmpty += ClosePanel;
        }

        private void ShowCommandsByType(SprintType type, Vector2 position)
        {
            _commandPanelView.gameObject.SetActive(true);
            _commandPanelView.ShowCommands(_commandManager.GetCommandsForSprint(type));
            UpdatePanelPosition(position);
            _localEvents.TriggerOpenPanel();
        }

        
        private void ClosePanel()
        {
            _localEvents.TriggerClosePanel();
            _commandPanelView.gameObject.SetActive(false);
        }
        
        private void UpdatePanelPosition(Vector3 worldPosition)
        {
            Vector2 screenPosition = _camera.WorldToScreenPoint(worldPosition);
            _commandPanelView.SetPosition(screenPosition);
        }

        public void CleanUp()
        {
            _localEvents.OnMouseClickIO -= ShowCommandsByType;
            _localEvents.OnClickEmpty -= ClosePanel;
        }
    }
}