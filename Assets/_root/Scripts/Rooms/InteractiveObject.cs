using System;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Rooms
{
    public class InteractiveObject : MonoBehaviour, IInteractiveObject
    {
        [SerializeField] private InteractiveObjectType _interactiveObjectType;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _rootObject;

        public SpriteRenderer spriteRenderer => _spriteRenderer;
        public Vector3 Position => GetComponent<Transform>().position;
        public Transform RootObjectPosition => _rootObject;

        public Action OnCursorEnter { get; set; }
        public Action OnCursorExit { get; set; }
        
        public InteractiveObjectType ObjectType => _interactiveObjectType;
        
        public void SetSortingOrder()
        {
            _spriteRenderer.sortingOrder = Consts.DafaultSortingOrder;
        }
    }
}