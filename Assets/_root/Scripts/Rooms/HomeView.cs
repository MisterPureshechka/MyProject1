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
        public Vector3 InitialPosition => _initialTransform.position;
        public float RoomSize => _roomCollider.bounds.size.x;
        
        public void SetSortingOrder()
        {
            GetComponent<SpriteRenderer>().sortingOrder = Consts.BackGroundSortingOrder;
        }
    }
}