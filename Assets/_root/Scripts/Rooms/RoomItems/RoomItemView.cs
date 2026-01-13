using _root.Scripts.Rooms.RoomItems;
using UnityEngine;

namespace Scripts.Rooms.RoomItems
{
    public class RoomItemView : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _walkToTransform;
        
        public Transform Root => _root;
        public Transform WalkToTransform => _walkToTransform;
        
        private RoomItem _roomItem;
        
        public void Init(RoomItem roomItem)
        {
            _roomItem = roomItem;
            _roomItem.View = this;
        }

    }
}