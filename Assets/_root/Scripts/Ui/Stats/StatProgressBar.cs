using System.Collections.Generic;
using UnityEngine;

internal class StatProgressBar : MonoBehaviour
{
    [SerializeField] private List<Bubble> _bubble;
    public void UpdateProgressBar(float norm01)
    {
        if (_bubble == null || _bubble.Count == 0) return;

        float clamped = Mathf.Clamp01(norm01);
        int count = _bubble.Count;
        float seg = 1f / count;

        for (int i = 0; i < count; i++)
        {
            float halfThreshold = (i + 0.5f) * seg - Mathf.Epsilon;
            float fullThreshold = (i + 1f) * seg - Mathf.Epsilon;
            
            if (clamped >= fullThreshold)
            {
                _bubble[i].SetFull();
            }
            else if (clamped >= halfThreshold)
            {
                _bubble[i].SetSpriteHalf(true);
            }
            else
            {
                _bubble[i].SetEmpty();
            }          
        }
    }
}