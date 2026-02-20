using _root.Planning;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using UnityEngine;
using System.Linq;

namespace Scripts.Ui
{
    public class NextStateController : IController
    {
        private readonly NextStateButton _nextStateButton;
        private readonly GameStateMachine _gameStateMachine;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LevelMapConfig _config;

        public NextStateController(
            NextStateButton nextStateButton,
            GameStateMachine gameStateMachine,
            ProgressDataAdapter progressDataAdapter,
            LevelMapConfig config)
        {
            _nextStateButton = nextStateButton;
            _gameStateMachine = gameStateMachine;
            _progressDataAdapter = progressDataAdapter;
            _config = config;

            _nextStateButton.Button.onClick.AddListener(OnClick);
            RefreshButtonState();
        }

        private void OnClick()
        {
            if (!TryGetNextNode(out var currentNodeId, out var nextNodeId, out var nextType))
                return;

            _progressDataAdapter.MarkRoadmapNodeCompleted(currentNodeId);

            _progressDataAdapter.SetCurrentRoadmapNodeId(nextNodeId);

            EnterState(nextType);
        }

        private bool TryGetNextNode(out string currentId, out string nextId, out NodeType nextType)
        {
            currentId = _progressDataAdapter.GetCurrentRoadmapNodeId();
            nextId = null;
            nextType = default;

            if (string.IsNullOrEmpty(currentId))
                return false;

            var curId = currentId; // <-- локальная копия, теперь можно в лямбде

            var currentNode = _config.LevelNodes.FirstOrDefault(n => n.Id == curId);
            if (currentNode == null || currentNode.NextNodeIds == null || currentNode.NextNodeIds.Count == 0)
                return false;

            nextId = currentNode.NextNodeIds[0];
            var nId = nextId;

            var nextNode = _config.LevelNodes.FirstOrDefault(n => n.Id == nId);
            if (nextNode == null)
                return false;

            nextType = nextNode.Type;
            return true;
        }



        private void RefreshButtonState()
        {
            _nextStateButton.gameObject.SetActive(LoadNextNodeTypeOrNull() != null);
        }

        private NodeType? LoadNextNodeTypeOrNull()
        {
            string currentId = _progressDataAdapter.GetCurrentRoadmapNodeId();
            if (string.IsNullOrEmpty(currentId))
                return null;

            var currentNode = _config.LevelNodes.FirstOrDefault(n => n.Id == currentId);
            if (currentNode == null)
                return null;

            if (currentNode.NextNodeIds == null || currentNode.NextNodeIds.Count == 0)
                return null;

            string nextId = currentNode.NextNodeIds[0];

            var nextNode = _config.LevelNodes.FirstOrDefault(n => n.Id == nextId);
            if (nextNode == null)
                return null;

            return nextNode.Type;
        }

        private void EnterState(NodeType type)
        {
            Object.Destroy(_nextStateButton.gameObject);
            
            switch (type)
            {
                case NodeType.Work:
                    _gameStateMachine.EnterState<WorkState>();
                    break;
                case NodeType.Build:
                    _gameStateMachine.EnterState<ShopState>();
                    break;
                case NodeType.Hire:
                    _gameStateMachine.EnterState<EmployeeHireState>();
                    break;
                case NodeType.Upgrade:
                    _gameStateMachine.EnterState<UpgradeState>();
                    break;
                case NodeType.OfficeUpgrade:
                    // _gameStateMachine.EnterState<OfficeUpgradeState>();
                    _gameStateMachine.EnterState<RoadMapState>();
                    break;
                default:
                    _gameStateMachine.EnterState<RoadMapState>();
                    break;
            }
        }

        public void CleanUp()
        {
            _nextStateButton?.Button?.onClick.RemoveAllListeners();
        }
    }
}
