using UnityEngine;

namespace Scripts.UpgradeLogic
{
    internal class WorkspaceView : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private SpriteRenderer _renderer;
        
        public void Upgrade(int id)
        {
            _renderer.sprite = _sprites[id];
        }
    }
}