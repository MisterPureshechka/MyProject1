using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Rooms
{
    public interface IRoomView : ISortedLayer
    {
        List<IInteractiveObject> InteractiveObjects { get; }
        Vector3 InitialPosition { get; }
        float RoomSize { get; }
    }
}