using UnityEngine;

namespace Scripts.UpgradeLogic
{
    public class ChairView : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private SpriteRenderer _renderer;
        public void Upgrade(int id)
        {
            _renderer.sprite = _sprites[id];
        }
    }
}