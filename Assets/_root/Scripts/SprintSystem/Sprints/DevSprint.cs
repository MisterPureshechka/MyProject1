using UnityEngine;

namespace Scripts.Tasks
{
    public class DevSprint : Sprint<IDevTask> 
    {
        private readonly Vector3 _worldPosition;
        public override bool ShouldPersistTasksOnExit => true;
        public override SprintType Type => SprintType.Dev;
        public DevSprint(int capacity, Vector3 worldPosition) : base(capacity)
        {
            _worldPosition = worldPosition;
        }
        
        public override bool HasCatalog => true;
        
    }
}