using UnityEngine;

namespace Scripts.Rooms
{
    public class SideRoom : MonoBehaviour, ISideRoom
    {
        [SerializeField] private bool _isLeftSide;
        public bool IsLeftRoom => _isLeftSide;
    }
}