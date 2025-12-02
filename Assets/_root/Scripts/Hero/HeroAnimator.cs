using System.Collections.Generic;
using Core;
using Scripts.Animator;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Hero
{
    public class HeroAnimator : IExecute, ICleanUp
    {
        private SpriteAnimator _spriteAnimator;
        private HeroConfig _heroConfig;
        
        private HeroView _heroView;
        private readonly LocalEvents _localEvents;

        private bool _isWithCoffee;
        private MoodState _bodyState;
        private MoodState _faceMoodState;
        private CleanState _cleanState;
        
        private Dictionary<(HeroAnimationState, MoodState), HeroBackHandSequence> _backHandIndex;
        private Dictionary<(HeroAnimationState, MoodState), HeroBodySequence> _bodyIndex;
        private Dictionary<(HeroAnimationState, MoodState), HeroEyesSequence> _eyesIndex;
        private Dictionary<(HeroAnimationState, CleanState), HeroHeadSequence> _headIndex;
        
        private HeroAnimationState _currentAnimState = HeroAnimationState.Idle;

        public HeroAnimator(HeroConfig heroConfig, HeroView heroView, LocalEvents localEvents)
        {
            _spriteAnimator = new SpriteAnimator();
            _heroConfig = heroConfig;
            _heroView = heroView;
            _localEvents = localEvents;
            
            BuildBackHandIndex();
            BuildBodyIndex();
            BuildHeadIndex();
            BuildEyesSequence();

            _localEvents.OnMoodStateChange += ChangeFaceMoodState;
            _localEvents.OnBodyStateChange += BodyStateChange;
            _localEvents.OnCleanStateChange += ChangeCleanState;
            _localEvents.OnTakeCoffee += ChangeCoffeeState;
        }

        private void ChangeCoffeeState(bool hasCoffee)
        {
            _isWithCoffee = hasCoffee;
            //StartAnimation(HeroAnimationState.Idle, true);
        }

        private void ChangeCleanState(CleanState state)
        {
            _cleanState = state;
        }

        private void BodyStateChange(MoodState state)
        {
            _bodyState = state;
        }

        private void ChangeFaceMoodState(MoodState state)
        {
            _faceMoodState = state;
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
            _headIndex = new Dictionary<(HeroAnimationState, CleanState), HeroHeadSequence>();
            foreach (var s in _heroConfig.HeadSequences)
                _headIndex[(s.HeroAnimationState, s.cleanState)] = s;
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
                if (_cleanState == CleanState.Dirty)
                {
                    _cleanState = CleanState.Clean;
                    StartAnimation(HeroAnimationState.Walk, true);
                    return;
                }
                else
                {
                    _cleanState = CleanState.Dirty;
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
                if (_bodyState == MoodState.Sad)
                {
                    _bodyState = MoodState.Normal;
                    Debug.Log($"_moodState changed to = {_bodyState}");
                    StartAnimation(HeroAnimationState.Walk, true);
                    return;
                }
            
                if (_bodyState == MoodState.Normal)
                {
                    _bodyState = MoodState.Happy;
                    Debug.Log($"_moodState changed to = {_bodyState}");
                    StartAnimation(HeroAnimationState.Walk, true);
                    return;
                }
            
                if (_bodyState == MoodState.Happy)
                {
                    _bodyState = MoodState.Sad;
                    Debug.Log($"_moodState changed to = {_bodyState}");
                    StartAnimation(HeroAnimationState.Walk, true);
                }
                
            }

           
        }

        public void ChangeHairState(CleanState state)
        {
            _cleanState = state;
        }

        private void IsWithCoffee(bool value)
        {
            _isWithCoffee = value;
        }

        public void StartAnimation(HeroAnimationState animationState, bool isLoop)
        {
            _currentAnimState = animationState;
            var baseSpeed = GetSpeedByType(animationState);
            ChangeEyesState(_currentAnimState, _faceMoodState, _bodyState, baseSpeed, isLoop);
            ChangeHeadState(_currentAnimState, _cleanState, baseSpeed, isLoop);
            ChangeBodyState(_currentAnimState, _bodyState, _faceMoodState, baseSpeed,  isLoop);
            ChangePantsState(_currentAnimState, baseSpeed, isLoop);
            ChangeBackHand(_currentAnimState, _bodyState, baseSpeed, isLoop);
        }

        private float GetSpeedByType(HeroAnimationState animationState)
        {
            switch (animationState)
            {
                case HeroAnimationState.Walk:
                    return _heroConfig.AnimationSpeed;
                case HeroAnimationState.Chill:
                    return _heroConfig.ChillSpeed;
                case HeroAnimationState.Eat:
                    return _heroConfig.EatSpeed;
                case HeroAnimationState.Idle:
                    return _heroConfig.IdleSpeed;
                case HeroAnimationState.Read:
                    return _heroConfig.ReadSpeed;
                case HeroAnimationState.Dev:
                    return _heroConfig.WorkSpeed;
                default:
                    return _heroConfig.AnimationSpeed;
            }
        }
        
        private void ChangeEyesState(HeroAnimationState animationState, MoodState moodState, MoodState bodyState, float animationSpeed, bool isLoop)
        {
            if (_heroConfig == null || _heroConfig.EyesSequences == null || _heroConfig.EyesSequences.Count == 0)
            {
                Debug.LogWarning("No Head sequences configured.");
                return;
            }
            
            _heroView.EyesSprite.gameObject.SetActive(true);

            if (_eyesIndex == null || _eyesIndex.Count == 0)
                BuildEyesSequence();

            if (bodyState == MoodState.Sad && moodState == MoodState.Happy)
            {
                moodState = MoodState.Normal;
            }

            if (!_eyesIndex.TryGetValue((animationState, moodState), out var sequence))
            {
                sequence = _heroConfig.EyesSequences.Find(s => s.HeroAnimationState == animationState);
                
                
                if (sequence == null)
                {
                    _heroView.EyesSprite.gameObject.SetActive(false);
                    Debug.LogWarning($"No Head sequence found for {animationState} / {moodState}");
                    return;
                }
            }

            _spriteAnimator.StartAnimation(_heroView.EyesSprite, sequence.Sprites, isLoop, animationSpeed);
        }

        private void ChangeHeadState(HeroAnimationState animationState, CleanState cleanState, float animationSpeed, bool isLoop)
        {
            Debug.LogWarning($"Head state changed to {animationState} / {cleanState}");
            
            if (_heroConfig == null || _heroConfig.HeadSequences == null || _heroConfig.HeadSequences.Count == 0)
            {
                Debug.LogWarning("No Head sequences configured.");
                return;
            }

            if (_headIndex == null || _headIndex.Count == 0)
                BuildHeadIndex();
            
            _heroView.HeadSprite.gameObject.SetActive(true);

            // if (cleanState == CleanState.SmellsLikeShit)
            // {
            //     cleanState = CleanState.Dirty;
            // }

            if (!_headIndex.TryGetValue((animationState, cleanState), out var sequence))
            {
                sequence = _heroConfig.HeadSequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    _heroView.HeadSprite.gameObject.SetActive(false);
                    return;
                }
            }

            _spriteAnimator.StartAnimation(_heroView.HeadSprite, sequence.Sprites, isLoop, animationSpeed);
        }
        
        private void ChangeBodyState(HeroAnimationState animationState, MoodState bodyState, MoodState moodState, float animationSpeed, bool isLoop)
        {
            if (_heroConfig == null || _heroConfig.BodySequences == null || _heroConfig.BodySequences.Count == 0)
            {
                return;
            }

            if (_bodyIndex == null || _bodyIndex.Count == 0)
                BuildBodyIndex();

            if (moodState == MoodState.Sad && bodyState == MoodState.Happy)
            {
                bodyState = MoodState.Normal;
            }

            if (!_bodyIndex.TryGetValue((animationState, bodyState), out var sequence))
            {
                sequence = _heroConfig.BodySequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    return;
                }
            }

            _spriteAnimator.StartAnimation(_heroView.BodySprite, sequence.Sprites, isLoop, animationSpeed);
        }
        
        private void ChangePantsState(HeroAnimationState animationState, float animationSpeed, bool isLoop)
        {
            var sequence = _heroConfig.PantsSequences.Find(s => s.HeroAnimationState == animationState);
            if (sequence != null)
            {
                _heroView.PantsSprite.gameObject.SetActive(true);
                _spriteAnimator.StartAnimation(_heroView.PantsSprite, sequence?.Sprites, isLoop, animationSpeed);
            }
            else
            {
                _heroView.PantsSprite.gameObject.SetActive(false);
            }
            
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
            
            _heroView.BackHandSprite.gameObject.SetActive(true);

            if (!_backHandIndex.TryGetValue((animationState, mood), out var sequence))
            {
                sequence = _heroConfig.BackHandSequences.Find(s => s.HeroAnimationState == animationState);

                if (sequence == null)
                {
                    _heroView.BackHandSprite.gameObject.SetActive(false);
                    return;
                }
            }

            var coffeeSprites = _heroConfig.BackHandWithCoffeeSprites;

            if (animationState == HeroAnimationState.Idle)
            {
                coffeeSprites = _heroConfig.IdleBackHandWithCoffeeSprites;
            }

            var resultSprites = _isWithCoffee && coffeeSprites != null &&
                                coffeeSprites.Count > 0
                ? coffeeSprites
                : sequence.Sprites;

            _spriteAnimator.StartAnimation(_heroView.BackHandSprite, resultSprites, isLoop, animationSpeed);
        }

        public void CleanUp()
        {
            _localEvents.OnMoodStateChange -= ChangeFaceMoodState;
            _localEvents.OnBodyStateChange -= BodyStateChange;
            _localEvents.OnCleanStateChange -= ChangeCleanState;
            _localEvents.OnTakeCoffee -= ChangeCoffeeState;
        }
    }

    
}