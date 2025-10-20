using Scripts.Utils;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroView : MonoBehaviour, ISortedLayer
    {
        [field: SerializeField] public SpriteRenderer EyesSprite { get; private set; }
        [field: SerializeField] public SpriteRenderer HeadSprite { get; private set; }
        [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }
        [field: SerializeField] public SpriteRenderer PantsSprite { get; private set; }
        [field: SerializeField] public SpriteRenderer BackHandSprite { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public ParticleSystem FX { get; private set; }
        
        public void FlipX(bool isLeft)
        {
            EyesSprite.flipX = isLeft;
            HeadSprite.flipX = isLeft;
            BodySprite.flipX = isLeft;
            PantsSprite.flipX = isLeft;
            BackHandSprite.flipX = isLeft;
        }

        public void SetSortingOrder()
        {
            // EyesSprite.sortingOrder = Consts.HeroSortingOrder;
            // HeadSprite.sortingOrder = Consts.HeroSortingOrder;
            // BodySprite.sortingOrder = Consts.HeroSortingOrder;
            // PantsSprite.sortingOrder = Consts.HeroSortingOrder;
            // BackHandSprite.sortingOrder = Consts.HeroSortingOrder;
        }
        
        public void SetSortingOrder(int sortingOrder)
        {
            EyesSprite.sortingOrder += sortingOrder;
            HeadSprite.sortingOrder += sortingOrder;
            BodySprite.sortingOrder += sortingOrder;
            PantsSprite.sortingOrder += sortingOrder;
            BackHandSprite.sortingOrder += sortingOrder;
        }

        public void EmitFx(float rate)
        {
            FX.emissionRate = rate;
        }
    }
}