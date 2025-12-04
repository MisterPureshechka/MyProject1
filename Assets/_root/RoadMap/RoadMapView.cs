using UnityEngine;

namespace _root.Planning
{
    public class RoadMapView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _startPos;

        public RectTransform Root => _root;
        public Vector2 StartPos => _startPos.anchoredPosition;
    }
}