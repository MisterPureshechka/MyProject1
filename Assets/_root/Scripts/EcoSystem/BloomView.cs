using System;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.EcoSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BloomView : MonoBehaviour
    {
        public System.Action<BloomView> OnHeroEnter;
        public System.Action<BloomView> OnHeroExit;
        
        [field: SerializeField] public SpriteRenderer SpriteRenderer;
        
        public bool IsHeroInside { get; private set; }

        private void Awake()
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.simulated = true; 
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Consts.PlayerKey))
            {
                IsHeroInside = true;
                OnHeroEnter?.Invoke(this);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Consts.PlayerKey))
            {
                IsHeroInside = false;
                OnHeroExit?.Invoke(this);
            }
        }
    }
}