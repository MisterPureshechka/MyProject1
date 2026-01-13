using UnityEngine;
using Scripts.Rooms.SlotLogic;

using UnityEngine;
using Scripts.Rooms.SlotLogic;

namespace Scripts.Rooms
{
    public class RoomView : MonoBehaviour
    {
        [field: SerializeField] public SlotView SlotPrefab { get; private set; }
        [field: SerializeField] public Transform SlotsRoot { get; private set; }
        [field: SerializeField] public float SlotSpacing { get; private set; } = 0.2f;
    }
}
