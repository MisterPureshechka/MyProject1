namespace Scripts.Progress
{
    public sealed class ProgressDataAdapter
    {
        private readonly SaveService _storage;
        private ProgressData _data;

        public ProgressDataAdapter(SaveService storage)
        {
            _storage = storage;
            Load();
        }
        
        public string GetCurrentRoadmapNodeId() => _data.CurrentRoadmapNodeId;

        public void SetCurrentRoadmapNodeId(string nodeId, bool save = true)
        {
            _data.CurrentRoadmapNodeId = nodeId;
            if (save) Save();
        }

        public bool IsRoadmapNodeCompleted(string nodeId)
        {
            return _data.CompletedRoadmapNodeIds != null &&
                   _data.CompletedRoadmapNodeIds.Contains(nodeId);
        }

        public void MarkRoadmapNodeCompleted(string nodeId, bool save = true)
        {
            if (_data.CompletedRoadmapNodeIds == null)
                _data.CompletedRoadmapNodeIds = new System.Collections.Generic.HashSet<string>();

            if (_data.CompletedRoadmapNodeIds.Add(nodeId) && save)
                Save();
        }

        public void Load()
        {
            _data = _storage.LoadProgress();
        }

        public void Save()
        {
            _storage.SaveProgress(_data);
        }

        public ProgressData Data => _data;
    }
}