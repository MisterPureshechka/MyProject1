using System;
using Scripts.Tasks;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Rooms
{
    public class InteractiveObject : MonoBehaviour, IInteractiveObject
    {
        [SerializeField] private SprintType _sprintType;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _rootObject;
        [SerializeField] private InteractiveObjectType _ioType;

        public SpriteRenderer spriteRenderer => _spriteRenderer;
        public SprintType SprintType => _sprintType;
        public Vector3 Position => GetComponent<Transform>().position;
        public Transform RootObjectPosition => _rootObject;

        public Action OnCursorEnter { get; set; }
        public Action OnCursorExit { get; set; }
        public InteractiveObjectType IOType => _ioType;

        public void SetSortingOrder()
        {
            _spriteRenderer.sortingOrder = Consts.DafaultSortingOrder;
        }
    }
}