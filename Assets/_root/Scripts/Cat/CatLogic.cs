using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using DG.Tweening;
using Scripts.Animator;
using Scripts.GlobalStateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Cat
{
    public class CatLogic : IExecute, ICleanUp
    {
        private LocalEvents _localEvents;
        private readonly Vector3 _initialPos;
        private CatStateMachine _catStateMachine;
        private CatView _catView;
        private CatPositionRegisterer _catPositionRegisterer;
        private List<CatTargetPosition> _allCatTargetPositions;
        private List<CatTargetPosition> _availableTargetPositions;
        private CatConfig _catConfig;
        private float _yPos;
        private CatTargetPosition _currentTargetPosition;
        private SpriteAnimator _spriteAnimator;
        private SpriteRenderer _catSpriteRenderer;

        public CatWalkState WalkState { get; private set; }
        public CatSeatState SeatState { get; private set; }
        public CatSleepState SleepState { get; private set; }
        public CatJumpState JumpState { get; private set; }
        public CatJumpDownState JumpDownState { get; private set; }

        public bool IsOnFloor => _isOnFloor;

        private bool _isOnFloor = true;
        private Sequence _sequence;

        public CatLogic(LocalEvents localEvents, Vector3 initialPos, CatView catView, CatPositionRegisterer catPositionRegisterer, CatConfig catConfig)
        {
            _localEvents = localEvents;
            _initialPos = initialPos;
            _catView = catView;
            _catPositionRegisterer = catPositionRegisterer;
            _catConfig = catConfig;
            _yPos = initialPos.y;
            _catSpriteRenderer = _catView.CatSprite;
            _allCatTargetPositions = _catPositionRegisterer.GetPositions().ToList();

            _catView.CatTransform.position = _initialPos;
            _spriteAnimator = new SpriteAnimator();
            
            _catStateMachine = new CatStateMachine();
            
            WalkState = new CatWalkState(this);
            SeatState = new CatSeatState(this);
            SleepState = new CatSleepState(this);
            JumpState = new CatJumpState(this);
            JumpDownState = new CatJumpDownState(this);
            
            _catStateMachine.Init(SleepState);
        }

        public void CleanUp()
        {
            
        }

        public void SetCatOnFloor(bool isOnFloor)
        {
            _isOnFloor = isOnFloor;
        }

        public void MoveCatToPosition(Vector3 moveToPosition, float deltaTime)
        {
            var newPosition = Vector3.MoveTowards(_catView.transform.position, moveToPosition, _catConfig.WalkSpeed * deltaTime);

            _catView.transform.position = newPosition;
        }
        
        public void JumpToPosition(Vector3 target, Action onComplete = null)
        {
            var duration = 0.4f;
            var jumpHeight = _catConfig.JumpHeight;

            _catView.transform.DOMoveX(target.x, duration);

            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_catView.transform.DOMoveY(target.y + jumpHeight, duration / 2).SetEase(Ease.OutQuad));
            _sequence.Append(_catView.transform.DOMoveY(target.y, duration / 2).SetEase(Ease.InQuad));
            _sequence.OnComplete( () => onComplete?.Invoke() );
        }
        
        public void PlayAnimation(CatAnimationState animationState, bool isLoop)
        {
            var sequence = _catConfig.Sequences.Find(s => s.CatAnimationState == animationState);
            
            if (sequence != null)
            {
                _spriteAnimator.StartAnimation(_catSpriteRenderer, sequence.Sprites, isLoop, sequence.Speed);
            }
            else
            {
                Debug.LogWarning($"Animation {animationState} not found");
            }
                
        }
        
        public void PlayTransitionAnimation(CatAnimationState from, CatAnimationState to, bool isLoop, Action onTransition = null, Action onComplete = null)
        {
            var fromSequence = _catConfig.Sequences.Find(f => f.CatAnimationState == from);
            var toSequence = _catConfig.Sequences.Find(t => t.CatAnimationState == to);

            _spriteAnimator.StartAnimation(_catSpriteRenderer, fromSequence.Sprites, false, fromSequence.Speed,
                () =>
                {
                    onTransition?.Invoke();
                    _spriteAnimator.StartAnimation(_catSpriteRenderer, toSequence.Sprites, isLoop, toSequence.Speed, 
                        () =>
                        {
                            onComplete?.Invoke();
                        });
                });
        }

        public void SetSortingOrder(int sortingOrder)
        {
            _catView.SetSortingOrder(sortingOrder);
        }
        
        public void JumpDownToPosition(Vector3 target, Action onComplete = null)
        {
            var startPos = _catView.transform.position;
            var duration = _catConfig.JumpDownDuration;
            var jumpHeight = _catConfig.JumpDownHeight;

            _catView.transform.DOMoveX(target.x, duration);

            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_catView.transform.DOMoveY(startPos.y + jumpHeight, duration / 2).SetEase(Ease.OutQuad));
            _sequence.Append(_catView.transform.DOMoveY(target.y, duration / 2).SetEase(Ease.InQuad));
            _sequence.OnComplete( () => onComplete?.Invoke() );
        }

        public void FlipSprite(bool isLeft)
        {
            _catView.CatSprite.flipX = isLeft;
        }
        
        public Vector3 NormalizeVector(Vector3 vector)
        {
            return new Vector3(vector.x, _yPos, 0);
        }

        public CatTargetPosition GetNewTargetPosition()
        {
            if (_allCatTargetPositions == null || _allCatTargetPositions.Count == 0)
            {
                Debug.LogWarning("Can't find cat target!");
                return null;
            }

            if (_availableTargetPositions == null || _availableTargetPositions.Count == 0)
            {
                _availableTargetPositions = new List<CatTargetPosition>(_allCatTargetPositions);
                _availableTargetPositions.Remove(_currentTargetPosition);
            }

            int index = Random.Range(0, _availableTargetPositions.Count);
            var newTarget = _availableTargetPositions[index];

            _availableTargetPositions.RemoveAt(index);

            _currentTargetPosition = newTarget;
            return newTarget;
        }

        public CatTargetPosition GetCurrentTargetPosition()
        {
            if (_currentTargetPosition == null)
            {
                GetNewTargetPosition();
            }
            
            return _currentTargetPosition;
        }

        public Vector3 GetCatPosition()
        {
            return _catView.CatTransform.position;
        }

        public void Execute(float deltatime)
        {
            _catStateMachine.CurrentState.Update(deltatime);
            _spriteAnimator.Execute(deltatime);
        }

        public void ChangeState(CatBaseState catNextState)
        {
            _catStateMachine.ChangeState(catNextState);
        }
    }
}