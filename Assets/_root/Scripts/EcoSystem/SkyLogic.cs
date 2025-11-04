using Core;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.EcoSystem
{
    public class SkyLogic : IExecute, ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private readonly SkyView _skyView;
        
        private LoopingMoverGroup _moverGroup;

        private float _starsSpeed;

        public SkyLogic(LoopingMoverGroup loopingMoverGroup, LocalEvents localEvents, SkyView skyView)
        {
            _localEvents = localEvents;
            _skyView = skyView;

            _moverGroup = loopingMoverGroup;

            _localEvents.OnDayTimeChange += ChangeSky;
            _localEvents.OnNormalizeNightTimeChange += ChangeStars;
            _localEvents.OnActiveSprint += SpeedUp;
            _localEvents.OnSprintExit += SpeedDown;
        }

        private void ChangeSky(float dayTime)
        {
            _skyView.UpdateSkyColor(dayTime);
            
        }

        private void ChangeStars(float nightTime)
        {
            _skyView.UpdateStars(nightTime);
           
        }

        public void CleanUp()
        {
            _localEvents.OnDayTimeChange -= ChangeSky;
            _localEvents.OnNormalizeNightTimeChange -= ChangeStars;
            _localEvents.OnActiveSprint -= SpeedUp;
            _localEvents.OnSprintExit -= SpeedDown;
        }

        public void Execute(float deltatime)
        {
            _moverGroup.MoveObjectsLoop(deltatime, _starsSpeed);
        }

        private void SpeedUp()
        {
            _starsSpeed = 0.05f;
        }

        private void SpeedDown()
        {
            _starsSpeed = 0.01f;
        }
        
    }
}