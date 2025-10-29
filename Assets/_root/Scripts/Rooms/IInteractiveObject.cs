using System;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Rooms
{
    public interface IInteractiveObject : ISortedLayer
    {
        SpriteRenderer SpriteRenderer { get; }
        SpriteRenderer OutLine { get; }
        SprintType SprintType { get; }
        Vector3 Position { get; }
        Transform RootObjectPosition { get; }
        Transform IOTransform  { get; }
        Action OnCursorEnter { get; set; }
        Action OnCursorExit { get; set; }
        
        InteractiveObjectType IOType { get; }
    }
}