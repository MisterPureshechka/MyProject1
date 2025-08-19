using System;
using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using SpriteRenderer = UnityEngine.SpriteRenderer;

namespace Scripts.Rooms
{
    public class HomeView : MonoBehaviour, IRoomView
    {
        [SerializeField] private Transform _initialTransform;
        [SerializeField] private Collider2D _roomCollider;
        [SerializeField] private List<MonoBehaviour> _interactiveObjects;
        [SerializeField] private SideRoom[] _sideRooms;

        public Transform Transform => transform;

        public List<IInteractiveObject> InteractiveObjects
        {
            get
            {
                var interactiveObjects = new List<IInteractiveObject>();
                
                foreach (var go in _interactiveObjects)
                {
                    if (go is IInteractiveObject interactiveObject)
                    {
                        interactiveObjects.Add(interactiveObject);
                    }
                }
                return interactiveObjects;
            }
        }
        public ISideRoom[] SideRooms => _sideRooms;

        public Vector3 InitialPosition => _initialTransform.position;
        public float RoomSize => _roomCollider.bounds.size.x;
        public Collider2D Collider => _roomCollider;
        
        public void SetSortingOrder()
        {
            GetComponent<SpriteRenderer>().sortingOrder = Consts.BackGroundSortingOrder;
        }
    }
}