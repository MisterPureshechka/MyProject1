using System.Collections.Generic;
using System.Linq;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using UnityEngine;

namespace _root.Planning
{
    public class LevelMapController : ICleanUp
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private LevelMapConfig _config;
        private RoadMapView _roadMapView;

        private LevelNodeView _levelNodePrefab;
        private ConnectorView _connectorPrefab;
        
        private Dictionary<string, LevelNode> _nodeById;
        private Dictionary<string, LevelNodeView> _nodeViewById;

        private Dictionary<string, int> _nodeDepth;
        private Dictionary<string, int> _nodeVisitIndex;
        private Dictionary<string, Vector2> _nodeUiPosition;
        
        private class Connection
        {
            public string fromId;
            public string toId;
            public ConnectorView view;
        }

        private readonly List<Connection> _connections = new List<Connection>();
        
        private LevelNode _currentNode;

        

        public LevelMapController(
            GameStateMachine gameStateMachine,
            ProgressDataAdapter progressDataAdapter,
            LevelMapConfig config,
            RoadMapView roadMapView,
            LevelNodeView levelNodePrefab,
            ConnectorView connectorPrefab)
        {
            _gameStateMachine = gameStateMachine;
            _progressDataAdapter = progressDataAdapter;
            _config = config;
            _roadMapView = roadMapView;
            _levelNodePrefab = levelNodePrefab;
            _connectorPrefab = connectorPrefab;

            BuildMap();

            string startNodeId = LoadCurrentLevelNodeId();

            if (string.IsNullOrEmpty(startNodeId) || !_nodeById.ContainsKey(startNodeId))
                startNodeId = _config.StartNodeId;

            SetCurrentNodeOnStart(startNodeId);
            MarkCurrentNodeCompleted();
        }

        private string LoadCurrentLevelNodeId()
        {
            return _progressDataAdapter.GetCurrentRoadmapNodeId();
        }

        private void BuildMap()
        {
            _nodeById = _config.LevelNodes.ToDictionary(n => n.Id, n => n);
            _nodeViewById = new Dictionary<string, LevelNodeView>();

            CalculateLayout();

            foreach (LevelNode node in _config.LevelNodes)
            {
                var view = Object.Instantiate(_levelNodePrefab, _roadMapView.Root);
                view.Init(node, this);

                var rect = (RectTransform)view.transform;
                rect.anchoredPosition = _nodeUiPosition[node.Id];

                _nodeViewById[node.Id] = view;
            }
            
            CreateConnections();
        }

        private void CalculateLayout()
        {
            _nodeDepth = new Dictionary<string, int>();
            _nodeVisitIndex = new Dictionary<string, int>();
            _nodeUiPosition = new Dictionary<string, Vector2>();

            var queue = new Queue<string>();
            string startId = _config.StartNodeId;

            _nodeDepth[startId] = 0;
            queue.Enqueue(startId);

            int visitIndex = 0;

            while (queue.Count > 0)
            {
                string nodeId = queue.Dequeue();
                int depth = _nodeDepth[nodeId];

                if (!_nodeVisitIndex.ContainsKey(nodeId))
                    _nodeVisitIndex[nodeId] = visitIndex++;

                LevelNode node = _nodeById[nodeId];

                if (node.NextNodeIds == null)
                    continue;

                foreach (string nextId in node.NextNodeIds)
                {
                    if (!_nodeDepth.ContainsKey(nextId))
                    {
                        _nodeDepth[nextId] = depth + 1;
                        queue.Enqueue(nextId);
                    }
                }
            }

            var groupsByDepth = _nodeDepth
                .GroupBy(pair => pair.Value)
                .OrderBy(group => group.Key); 

            float xSpacing = 100f;
            float ySpacing = 75f;

            foreach (var group in groupsByDepth)
            {
                int depth = group.Key;

                var nodeIdsInColumn = group
                    .Select(pair => pair.Key)
                    .OrderBy(id => _nodeVisitIndex[id])
                    .ToList();

                int count = nodeIdsInColumn.Count;

                for (int i = 0; i < count; i++)
                {
                    string id = nodeIdsInColumn[i];

                    float x = depth * xSpacing;

                    float y = (i - (count - 1) * 0.5f) * -ySpacing;

                    _nodeUiPosition[id] = new Vector2(x, y);
                }
            }
        }
        
        public void OnNodeClicked(LevelNodeView view)
        {
            if (!IsNodeAvailable(view.LevelNodeData)) 
                return;

            LevelNode targetNode = view.LevelNodeData;

            Connection connection = _connections.FirstOrDefault(c =>
                c.fromId == _currentNode.Id &&
                c.toId == targetNode.Id);

            void FinishTransition()
            {
                _progressDataAdapter.MarkRoadmapNodeCompleted(_currentNode.Id);

                _progressDataAdapter.SetCurrentRoadmapNodeId(targetNode.Id);

                EnterState(targetNode.Type);
            }

            if (connection != null)
            {
                connection.view.PlaySelectAnimation(FinishTransition);
            }
            else
            {
                FinishTransition();
            }
        }

        private void EnterState(NodeType type)
        {
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
                default:
                    _gameStateMachine.EnterState<RoadMapState>();
                    break;
            }
        }
        
        public void MarkCurrentNodeCompleted()
        {
            _progressDataAdapter.MarkRoadmapNodeCompleted(_currentNode.Id);
            UpdateNodeVisualState();
        }
        
        private bool IsNodeAvailable(LevelNode node)
        {
            return _currentNode.NextNodeIds.Contains(node.Id);
        }
        
        private void SetCurrentNode(string nodeId)
        {
            _currentNode = _nodeById[nodeId];

            foreach (var pair in _nodeViewById)
            {
                var isCurrent = pair.Key == nodeId;
                pair.Value.SetCurrent(isCurrent);
            }

            UpdateNodeVisualState();
        }
        
        private void SetCurrentNodeOnStart(string nodeId)
        {
            _currentNode = _nodeById[nodeId];

            foreach (var pair in _nodeViewById)
            {
                var isCurrent = pair.Key == nodeId;
                pair.Value.SetCurrent(isCurrent);
            }

            UpdateNodeVisualState();
        }
        
        private void UpdateNodeVisualState()
        {
            foreach (var pair in _nodeViewById)
            {
                string nodeId = pair.Key;
                LevelNodeView view = pair.Value;
                LevelNode node = _nodeById[nodeId];

                bool isCurrent   = nodeId == _currentNode.Id;
                bool isCompleted = _progressDataAdapter.IsRoadmapNodeCompleted(nodeId);
                bool isNext      = _currentNode.NextNodeIds.Contains(nodeId);

                NodeViewState state;

                if (isCurrent && isCompleted)
                {
                    state = NodeViewState.Completed;
                }
                else if (isCurrent)
                {
                    state = NodeViewState.Current;
                }
                else if (isCompleted)
                {
                    state = NodeViewState.Completed;
                }
                else if (isNext)
                {
                    state = NodeViewState.Available;
                }
                else
                {
                    state = NodeViewState.Locked;
                }

                view.SetState(state);
            }

            UpdateConnectionsVisualState();
        }


        
        private void CreateConnections()
        {
            foreach (LevelNode node in _config.LevelNodes)
            {
                if (node.NextNodeIds == null)
                    continue;

                foreach (string nextId in node.NextNodeIds)
                {
                    var from = (RectTransform)_nodeViewById[node.Id].transform;
                    var to   = (RectTransform)_nodeViewById[nextId].transform;

                    CreateLineBetween(node.Id, nextId, from, to);
                }
            }
        }

        private void CreateLineBetween(string fromId, string toId, RectTransform fromNode, RectTransform toNode)
        {
            var line = Object.Instantiate(_connectorPrefab, _roadMapView.Root);
            line.transform.SetAsFirstSibling();

            Vector2 start = fromNode.anchoredPosition;
            Vector2 end = toNode.anchoredPosition;

            Vector2 diff = end - start;
            float length = diff.magnitude;

            var rect = line.Image.rectTransform;
            rect.anchoredPosition = (start + end) * 0.5f; 
            rect.sizeDelta = new Vector2(length, 3);

            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            rect.localRotation = Quaternion.Euler(0, 0, angle);

            // сохраняем коннектор
            _connections.Add(new Connection
            {
                fromId = fromId,
                toId = toId,
                view = line
            });
        }
        
        private void UpdateConnectionsVisualState()
        {
            foreach (var connection in _connections)
            {
                bool toCompleted = _progressDataAdapter.IsRoadmapNodeCompleted(connection.toId);
                bool fromCompleted = _progressDataAdapter.IsRoadmapNodeCompleted(connection.fromId);

                bool isNextFromCurrent =
                    connection.fromId == _currentNode.Id &&
                    _currentNode.NextNodeIds.Contains(connection.toId);

                bool isOnPassedPath =
                    fromCompleted &&
                    (toCompleted || connection.toId == _currentNode.Id);

                ConnectorView.ConnectionState state;

                if (isOnPassedPath)
                {
                    state = ConnectorView.ConnectionState.Completed;
                }
                else if (isNextFromCurrent)
                {
                    state = ConnectorView.ConnectionState.Available;
                }
                else
                {
                    state = ConnectorView.ConnectionState.Inactive;
                }

                connection.view.SetState(state);
            }
        }
        
        private void RunNodeLogic(LevelNode node)
        {
            switch (node.Type)
            {
                case NodeType.Work:
                    Debug.Log("Run Work node");
                    break;
                case NodeType.Hire:
                    Debug.Log("Run Hire node");
                    break;
                case NodeType.Build:
                    Debug.Log("Run Build node");
                    break;
                case NodeType.Upgrade:
                    Debug.Log("Run Upgrade node");
                    break;
                case NodeType.Perks:
                    Debug.Log("Run Perks node");
                    break;
                case NodeType.Release:
                    Debug.Log("Run Release node");
                    break;
                case NodeType.OfficeUpgrade:
                    Debug.Log("Run OfficeUpgrade node");
                    break;
            }
        }

        public void CleanUp()
        {
            Object.Destroy(_roadMapView.gameObject);
        }
    }
}