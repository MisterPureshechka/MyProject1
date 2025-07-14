using UnityEngine;

namespace Scripts.EcoSystem
{
    public class SkyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _skyRenderer;
        [SerializeField] private Color _dayColor;
        [SerializeField] private Color _nightColor;
        
        
        public void UpdateSkyColor(float value)
        {
            _skyRenderer.color = Color.Lerp(_nightColor, _dayColor, value);
        }
    }
}