using UnityEngine;

namespace Scripts.Cat
{
    public class CatView : MonoBehaviour
    {
        [SerializeField] private Transform _catTransform;
        [SerializeField] private SpriteRenderer _catSprite;

        public Transform CatTransform => _catTransform;

        public SpriteRenderer CatSprite => _catSprite;

        public void SetSortingOrder(int sortingOrder)
        {
            CatSprite.sortingOrder = sortingOrder;
        }
    }
}