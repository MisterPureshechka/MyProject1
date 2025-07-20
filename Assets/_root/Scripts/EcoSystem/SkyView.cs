using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Scripts.EcoSystem
{
    public class SkyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _starsPrefabs;
        [SerializeField] private SpriteRenderer _skyRenderer;
        [SerializeField] private Color _dayColor;
        [SerializeField] private Color _nightColor;
        [SerializeField] private Color _starDayColor;

        public SpriteRenderer[] StarsPrefabs => _starsPrefabs;


        public void UpdateSkyColor(float value)
        {
            _skyRenderer.color = Color.Lerp(_nightColor, _dayColor, value);
            
        }

        public void UpdateStars(float value)
        {
            foreach (var star in _starsPrefabs)
            {
                star.color = Color.Lerp(_starDayColor, Color.white, value);
            }
        }
    }
}