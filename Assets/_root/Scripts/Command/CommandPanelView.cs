using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CommandPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _commandButtonPrefab;
        [SerializeField] private Transform _commandsContainer;
        [SerializeField] private RectTransform _panelRect;

        [SerializeField] private Vector2 _offset = new Vector2(0f, 400f);
        
        public void ShowCommands(List<Command> commands)
        {
            ClearCommands();
        
            foreach (var command in commands)
            {
                var button = Instantiate(_commandButtonPrefab, _commandsContainer);
                var buttonComp = button.GetComponent<CommandButtonView>();
                buttonComp.Init(command.CommandName, command.OnExecute);
            }
        }

        private void ClearCommands()
        {
            foreach (Transform child in _commandsContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        public void SetPosition(Vector2 screenPosition)
        {
            _panelRect.position = screenPosition;
        
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(screenPosition.x, _panelRect.rect.width/2, Screen.width - _panelRect.rect.width/2),
                Mathf.Clamp(screenPosition.y, _panelRect.rect.height/2, Screen.height - _panelRect.rect.height/2)
            );
        
            _panelRect.position = clampedPosition + _offset;
        }
    }
}
