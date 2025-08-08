using UnityEngine;

namespace Scripts.Upgrade
{
    public class ChairView : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private SpriteRenderer _renderer;
        public void Upgrade(int id)
        {
            Debug.Log($"Chair upgraded to ID {id}");
            _renderer.sprite = _sprites[id];
        }
    }
}