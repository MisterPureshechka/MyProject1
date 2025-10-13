using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Scripts.Hero;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "ScriptableObjects/HeroConfig", order = 1)]
    public class HeroConfig : ScriptableObject
    {
        [field: SerializeField] public float WalkSpeed { get; private set; }
        [field: SerializeField] public float AnimationSpeed { get; private set; }
        [field: SerializeField] public List<HeroSpriteSequence> Sequences { get; private set; }
        [field: SerializeField] public List<HeroEyesSequence> EyesSequences { get; private set; }
        [field: SerializeField] public List<HeroHeadSequence> HeadSequences { get; private set; }
        [field: SerializeField] public List<HeroBodySequence> BodySequences { get; private set; }
        [field: SerializeField] public List<HeroBodySequence> PantsSequences { get; private set; }
        [field: SerializeField] public List<HeroBackHandSequence> BackHandSequences { get; private set; }
        
        [field: SerializeField] public List<Sprite> BackHandWithCoffeeSprites;
    }

    [Serializable]
    public sealed class HeroSpriteSequence
    {
        public HeroAnimationState HeroAnimationState;
        public List<Sprite> Sprites;
    }

    [Serializable]
    public sealed class HeroBodySequence
    {
        public HeroAnimationState HeroAnimationState;
        public MoodState MoodState;
        public List<Sprite> Sprites;
    }

    [Serializable]
    public sealed class HeroHeadSequence
    {
        public HeroAnimationState HeroAnimationState;
        public HeadState HeadState;
        public List<Sprite> Sprites;
    }
    
    [Serializable]
    public sealed class HeroEyesSequence
    {
        public HeroAnimationState HeroAnimationState;
        [FormerlySerializedAs("HeadState")] public MoodState MoodState;
        public List<Sprite> Sprites;
    }

    
    [Serializable]
    public sealed class HeroPartSequence
    {
        public HeroAnimationState HeroAnimationState;
        public List<Sprite> Sprites;
    }
    
    [Serializable]
    public sealed class HeroBackHandSequence
    {
        public HeroAnimationState HeroAnimationState;
        public MoodState BackHandState;
        public List<Sprite> Sprites;
    }
    
    public enum HeadState
    {
        Dirty,
        Clean,
    }

    public enum MoodState
    {
        Happy,
        Sad,
        Normal
    }

    public enum BackHandState
    {
        Tyred,
        Happy,
        Normal,
        Coffee
    }
}