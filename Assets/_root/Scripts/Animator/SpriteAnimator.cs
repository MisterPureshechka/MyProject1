using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Scripts.Animator
{
    public class SpriteAnimator : IExecute, ICleanUp
    {
        private sealed class Animation
        {
            public Action OnAnimationEnd;
            public List<Sprite> Sprites;
            public bool Loop;
            public float Speed = 10.0f;
            public float Counter = 0;
            public bool Sleep;

            

            public void UpdateAnimation(float deltatime)
            {
                if (Sleep) return;
                
                Counter += deltatime * Speed;

                if (Loop)
                {
                    while (Counter > Sprites.Count)
                    {
                        Counter -= Sprites.Count;
                    }

                }
                else if (Counter > Sprites.Count)
                {
                    Counter = 0;
                    Sleep = true;
                    OnAnimationEnd?.Invoke();
                }
            }
        }

        private Dictionary<SpriteRenderer, Animation> _activeSpriteAnimation = new Dictionary<SpriteRenderer, Animation>();


        public void StartAnimation(SpriteRenderer spriteRenderer, List<Sprite> sprites, bool loop, float speed, Action onAnimationEnd = null)
        {
            if (_activeSpriteAnimation.TryGetValue(spriteRenderer, out var animation))
            {
                animation.Sprites = sprites;
                animation.Counter = 0;
                animation.Sleep = false;
                animation.Loop = loop;
                animation.Speed = speed;
                animation.OnAnimationEnd = onAnimationEnd;
            }
            else
            {
                _activeSpriteAnimation.Add(spriteRenderer, new Animation()
                {
                    Sprites = sprites,
                    Loop = loop,
                    Speed = speed,
                    OnAnimationEnd = onAnimationEnd
                });
            }
        }

        public void Execute(float deltatime)
        {
            foreach (var animation in _activeSpriteAnimation)
            {
                animation.Value.UpdateAnimation(deltatime);
                
                if (animation.Value.Counter < animation.Value.Sprites.Count)
                {
                    animation.Key.sprite = animation.Value.Sprites[(int)animation.Value.Counter];
                }
            }
        }

        public void CleanUp()
        {
            _activeSpriteAnimation.Clear();
        }
    }
}