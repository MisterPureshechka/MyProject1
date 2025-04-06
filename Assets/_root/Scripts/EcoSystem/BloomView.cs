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

        private void Awake()
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.simulated = true; // Важно для Unity 6!
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Consts.PlayerKey))
            {
                OnHeroEnter?.Invoke(this);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Consts.PlayerKey))
            {
                OnHeroExit?.Invoke(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<Collider2D>().bounds.size);
        }
    }
}