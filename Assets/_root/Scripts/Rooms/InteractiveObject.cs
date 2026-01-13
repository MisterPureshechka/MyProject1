using System;
using Scripts.Rooms.SlotLogic;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Rooms
{
    public class InteractiveObject : MonoBehaviour, IInteractiveObject
    {
        [SerializeField] private SprintType _sprintType;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [FormerlySerializedAs("_spriteRenderer")] [SerializeField] private SpriteRenderer _outLine;
        [SerializeField] private Transform _rootObject;
        [SerializeField] private InteractiveObjectType _ioType;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public SpriteRenderer OutLine => _outLine;
        public SprintType SprintType => _sprintType;
        public Vector3 Position => GetComponent<Transform>().position;
        public Transform RootObjectPosition => _rootObject;
        public Transform IOTransform => gameObject.transform;
        public Action OnCursorEnter { get; set; }
        public Action OnCursorExit { get; set; }
        public InteractiveObjectType IOType => _ioType;

        public void SetSortingOrder()
        {
            _outLine.sortingOrder = Consts.DafaultSortingOrder;
        }
    }
}