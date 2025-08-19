using UnityEngine;

namespace Scripts.Rooms
{
    public interface ISideRoom
    {
        bool IsLeftRoom { get; }
        Collider2D Collider { get; }
    }
}