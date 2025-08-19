using UnityEngine;

namespace Scripts.Rooms
{
    public class SideRoom : MonoBehaviour, ISideRoom
    {
        [SerializeField] private bool _isLeftSide;
        [SerializeField] private Collider2D _collider;
        public bool IsLeftRoom => _isLeftSide;
        public Collider2D Collider => _collider;
    }
}