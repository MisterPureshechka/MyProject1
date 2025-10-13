using System.Collections.Generic;
using Core;
using Scripts.Animator;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroAnimator : IExecute
    {
        private SpriteAnimator _spriteAnimator;
        private HeroConfig _heroConfig;
        
        private HeroView _heroView;

        private bool _isWithCoffee;
        private MoodState _moodState;
        private HeadState _headState;
        
        private Dictionary<(HeroAnimationState, MoodState), HeroBackHandSequence> _backHandIndex;
        private Dictionary<(HeroAnimationState, MoodState), HeroBodySequence> _bodyIndex;
        private Dictionary<(HeroAnimationState, MoodState), HeroEyesSequence> _eyesIndex;
        private Dictionary<(HeroAnimationState, HeadState), HeroHeadSequence> _headIndex;

        public HeroAnimator(HeroConfig heroConfig, HeroView heroView)
        {
            _spriteAnimator = new SpriteAnimator();
            _heroConfig = heroConfig;
            _heroView = heroView;

            _moodState = MoodState.Sad;
            _headState = HeadState.Dirty;
            _isWithCoffee = true;
            
            Debug.Log($"_moodState = {_moodState}");
            Debug.Log($"_headState = {_headState}");
            
            BuildBackHandIndex();
            BuildBodyIndex();
            BuildHeadIndex();
            BuildEyesSequence();
        }
        
        private void BuildEyesSequence()
        {
            _eyesIndex = new Dictionary<(HeroAnimationState, MoodState), HeroEyesSequence>();
            foreach (var s in _heroConfig.EyesSequences)
                _eyesIndex[(s.HeroAnimationState, s.MoodState)] = s;
        }

        private void BuildBackHandIndex()
        {
            _backHandIndex = new Dictionary<(HeroAnimationState, MoodState), HeroBackHandSequence>();
            foreach (var s in _heroConfig.BackHandSequences)
                _backHandIndex[(s.HeroAnimationState, s.BackHandState)] = s;
        }
        
        private void BuildHeadIndex()
        {
            _headIndex = new Dictionary<(HeroAnimationState, HeadState), HeroHeadSequence>();
            foreach (var s in _heroConfig.HeadSequences)
                _headIndex[(s.HeroAnimationState, s.HeadState)] = s;
        }

        private void BuildBodyIndex()
        {
            _bodyIndex = new Dictionary<(HeroAnimationState, MoodState), HeroBodySequence>();
            foreach (var s in _heroConfig.BodySequences)
                _bodyIndex[(s.HeroAnimationState, s.MoodState)] = s;
        }
        
        public void Execute(float deltatime)
        {
            _spriteAnimator.Execute(deltatime);
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (_headState == HeadState.Dirty)
                {
                    _headState = HeadState.Clean;
                    StartAnimation(HeroAnimationState.Walk, true);
                    return;
                }
                else
                {
                    _headState = HeadState.Dirty;
                    StartAnimation(HeroAnimationState.Walk, true);
                }
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (_isWithCoffee)
                {
                    _isWithCoffee = false;
                }
                else
                {
                    _isWithCoffee = true;
                }
                
                StartAnimation(HeroAnimationState.Walk, true);
                Debug.Log($"_isWith coffee changed to = {_isWithCoffee}");
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (_moodState == MoodState.Sad)
                {
                    _moodState = MoodState.Normal;
                    Debug.Log($"_moodState changed to = {_moodState}");
                    StartAnimation(HeroAnimationState.Walk, true);
                    return;
                }

                if (_moodState == MoodState.Normal)
                {
                    _moodState = MoodState.Happy;
                    Debug.Log($"_moodState changed to = {_moodState}");
                    StartAnimation(HeroAnimationState.Walk, true);
                    return;
                }

                if (_moodState == MoodState.Happy)
                {
                    _moodState = MoodState.Sad;
                    Debug.Log($"_moodState changed to = {_moodState}");
                    StartAnimation(HeroAnimationState.Walk, true);
                }
                
            }

           
        }

        public void ChangeHairState(HeadState state)
        {
            _headState = state;
        }

        private void IsWithCoffee(bool value)
        {
            _isWithCoffee = value;
        }

        public void StartAnimation(HeroAnimationState animationState, bool isLoop)
        {
            var baseSpeed = _heroConfig.AnimationSpeed;
            ChangeEyesState(animationState, _moodState, baseSpeed, isLoop);
            ChangeHeadState(animationState, _headState, baseSpeed, isLoop);
            ChangeBodyState(animationState, _moodState, baseSpeed,  isLoop);
            ChangePantsState(animationState, baseSpeed, isLoop);
            ChangeBackHand(animationState, _moodState, baseSpeed, isLoop);
        }
        
        private void ChangeEyesState(HeroAnimationState animationState, MoodState moodState, float animationSpeed, bool isLoop)
        {
            if (_heroConfig == null || _heroConfig.EyesSequences == null || _heroConfig.EyesSequences.Count == 0)
            {
                Debug.LogWarning("No Head sequences configured.");
                return;
            }

            if (_eyesIndex == null || _eyesIndex.Count == 0)
                BuildHeadIndex();

            if (!_eyesIndex.TryGetValue((animationState, moodState), out var sequence))
            {
                sequence = _heroConfig.EyesSequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    Debug.LogWarning($"No Head sequence found for {animationState} / {moodState}");
                    return;
                }
            }

            _spriteAnimator.StartAnimation(_heroView.EyesSprite, sequence.Sprites, isLoop, animationSpeed);
        }

        private void ChangeHeadState(HeroAnimationState animationState, HeadState headState, float animationSpeed, bool isLoop)
        {
            if (_heroConfig == null || _heroConfig.HeadSequences == null || _heroConfig.HeadSequences.Count == 0)
            {
                Debug.LogWarning("No Head sequences configured.");
                return;
            }

            if (_headIndex == null || _headIndex.Count == 0)
                BuildHeadIndex();

            if (!_headIndex.TryGetValue((animationState, headState), out var sequence))
            {
                sequence = _heroConfig.HeadSequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    Debug.LogWarning($"No Head sequence found for {animationState} / {headState}");
                    return;
                }
            }

            _spriteAnimator.StartAnimation(_heroView.HeadSprite, sequence.Sprites, isLoop, animationSpeed);
        }
        
        private void ChangeBodyState(HeroAnimationState animationState, MoodState moodState, float animationSpeed, bool isLoop)
        {
            if (_heroConfig == null || _heroConfig.BodySequences == null || _heroConfig.BodySequences.Count == 0)
            {
                Debug.LogWarning("No Head sequences configured.");
                return;
            }

            if (_bodyIndex == null || _bodyIndex.Count == 0)
                BuildHeadIndex();

            if (!_bodyIndex.TryGetValue((animationState, moodState), out var sequence))
            {
                sequence = _heroConfig.BodySequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    Debug.LogWarning($"No Head sequence found for {animationState} / {moodState}");
                    return;
                }
            }

            _spriteAnimator.StartAnimation(_heroView.BodySprite, sequence.Sprites, isLoop, animationSpeed);
        }
        
        private void ChangePantsState(HeroAnimationState animationState, float animationSpeed, bool isLoop)
        {
            var sequence = _heroConfig.PantsSequences.Find(s => s.HeroAnimationState == animationState);
            if (sequence != null)
                _spriteAnimator.StartAnimation(_heroView.PantsSprite, sequence?.Sprites, isLoop, animationSpeed);
            if (sequence == null) Debug.LogWarning($"No sequence found for {animationState}");
        }
        
        private void ChangeBackHand(HeroAnimationState animationState, MoodState mood, float animationSpeed, bool isLoop)
        {
            if (_heroConfig == null || _heroConfig.BackHandSequences == null || _heroConfig.BackHandSequences.Count == 0)
            {
                Debug.LogWarning("No BackHand sequences configured.");
                return;
            }

            if (_backHandIndex == null || _backHandIndex.Count == 0)
                BuildBackHandIndex();

            if (!_backHandIndex.TryGetValue((animationState, mood), out var sequence))
            {
                sequence = _heroConfig.BackHandSequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    Debug.LogWarning($"No BackHand sequence found for {animationState} / {mood}");
                    return;
                }
            }

            var resultSprites = _isWithCoffee && _heroConfig.BackHandWithCoffeeSprites != null &&
                                _heroConfig.BackHandWithCoffeeSprites.Count > 0
                ? _heroConfig.BackHandWithCoffeeSprites
                : sequence.Sprites;

            _spriteAnimator.StartAnimation(_heroView.BackHandSprite, resultSprites, isLoop, animationSpeed);
        }

    }

    
}