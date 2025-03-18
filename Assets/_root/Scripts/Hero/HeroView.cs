using Scripts.Utils;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroView : MonoBehaviour, ISortedLayer
    {
        [field: SerializeField] public SpriteRenderer HeroSprite { get; private set; }

        public void SetSortingOrder()
        {
            HeroSprite.sortingOrder = Consts.HeroSortingOrder;
        }
    }
}