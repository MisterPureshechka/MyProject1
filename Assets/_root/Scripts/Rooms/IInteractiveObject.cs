using System;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Rooms
{
    public interface IInteractiveObject : ISortedLayer
    {
        SpriteRenderer spriteRenderer { get; }
        InteractiveObjectType ObjectType { get; }
        Vector3 Position { get; }

        Action OnCursorEnter { get; set; }
        Action OnCursorExit { get; set; }
    }
}