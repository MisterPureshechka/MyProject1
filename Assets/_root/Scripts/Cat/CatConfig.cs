using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Cat
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cat")]
    public class CatConfig : ScriptableObject
    {
        [field: SerializeField] public  List<CatSpriteSequence> Sequences { get; private set; }
        [field: SerializeField] public float WalkSpeed { get; set; }
        [field: SerializeField] public float JumpSpeed { get; set; }
        [field: SerializeField] public float JumpHeight { get; set; }
        [field: SerializeField] public float JumpDownDuration { get; set; } 
        [field: SerializeField] public float JumpDownHeight { get; set; }
    }
    
    [Serializable]
    public sealed class CatSpriteSequence
    {
        [FormerlySerializedAs("HeroAnimationState")] public CatAnimationState CatAnimationState;
        public List<Sprite> Sprites;
        public float Speed;
    }
}