using System.Collections.Generic;
using Scripts.Progress;
using Scripts.Rooms;
using UnityEngine;

namespace Scripts.Hero
{
    public class InitialPosLoader
    {
        private ProgressDataAdapterOLD _progressDataAdapterOld;
        private readonly InteractiveObjectRegisterer _registerer;


        private Dictionary<HeroStateId, Vector3> _heroPosMap;

        private Vector3 _initialPosition;
        
        public InitialPosLoader(ProgressDataAdapterOLD progressDataAdapterOld, InteractiveObjectRegisterer registerer)
        {
            _progressDataAdapterOld = progressDataAdapterOld;
            _registerer = registerer;
        }

        private void LoadInitialPosition()
        {
            
        }

        private void SetPosMap()
        {
            // _heroPosMap = new Dictionary<HeroStateId, Vector3>
            // {
            //     [HeroStateId.Walk] = WalkState,
            //     [HeroStateId.WalkToSprint] = WalkToSprintState,
            //     [HeroStateId.WalkToIO] = WalkToIOState,
            //     [HeroStateId.WalkToExit] = WalkToExitState,
            //     [HeroStateId.WalkToRootIO] = WalkToRootIOState,
            //     [HeroStateId.WalkToBed] = WalkToBedState,
            //     [HeroStateId.Dev] = DevState,
            //     [HeroStateId.Eat] = EatState,
            //     [HeroStateId.Sleep] = SleepState,
            //     [HeroStateId.Play] = PlayState,
            //     [HeroStateId.Read] = ReadState,
            //     [HeroStateId.Chill] = ChillState,
            //     [HeroStateId.Await] = HeroAwaitState,
            //     [HeroStateId.Toilet] = HeroToiletState,
            //     [HeroStateId.Bath] = HeroBathState,
            // };
        }
    }
}