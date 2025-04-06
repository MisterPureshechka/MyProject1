using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Scripts.Hero;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "ScriptableObjects/HeroConfig", order = 1)]
    public class HeroConfig : ScriptableObject
    {
        [field: SerializeField] public float WalkSpeed { get; private set; }
        
        [field: SerializeField] public List<HeroSpriteSequence> Sequences { get; private set; }
    }

    [Serializable]
    public sealed class HeroSpriteSequence
    {
        public HeroAnimationState HeroAnimationState;
        public List<Sprite> Sprites;
        public float Speed;
    }
}