using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Rooms
{
    public interface IRoomViewOLD : ISortedLayer
    {
        Transform Transform { get; }
        List<IInteractiveObject> InteractiveObjects { get; }
        ISideRoom[] SideRooms { get; }
        Vector3 InitialPosition { get; }
        float RoomSize { get; }
        Collider2D Collider { get; }
    }
}