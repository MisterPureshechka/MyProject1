using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Perks
{
    [CreateAssetMenu(fileName = "PerksSprites", menuName = "ScriptableObjects/Perks/Sprites")]
    public class PerksSprites : ScriptableObject
    {
        public List<PerkSpriteData> PerkSprites;
    }

    [System.Serializable]
    public struct PerkSpriteData
    {
        public string Name;
        public Sprite Sprite;
    }
}