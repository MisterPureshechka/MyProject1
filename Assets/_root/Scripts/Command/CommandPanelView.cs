using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Tasks
{
    public class CommandPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _commandButtonPrefab;
        [SerializeField] private Transform _commandsContainer;
        [SerializeField] private RectTransform _panelRect;

        [SerializeField] private Vector2 _offset = new Vector2(0f, 0);
        
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
        
        public void SetPosition(Vector2 screenPosition, Camera camera)
        {
            screenPosition += _offset;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _panelRect.parent as RectTransform,
                screenPosition,
                camera,
                out Vector2 localPoint
            );

            _panelRect.localPosition = ClampToCanvas(localPoint);
        }

        private Vector2 ClampToCanvas(Vector2 localPos)
        {
            RectTransform parentRect = _panelRect.parent as RectTransform;

            Vector2 halfSize = _panelRect.rect.size * 0.5f;
            Vector2 parentHalfSize = parentRect.rect.size * 0.5f;

            float x = Mathf.Clamp(localPos.x, -parentHalfSize.x + halfSize.x, parentHalfSize.x - halfSize.x);
            float y = Mathf.Clamp(localPos.y, -parentHalfSize.y + halfSize.y, parentHalfSize.y - halfSize.y);

            return new Vector2(x, y);
        }

    }
}
