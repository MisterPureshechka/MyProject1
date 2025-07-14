using Core;
using Scripts.GlobalStateMachine;

namespace Scripts.EcoSystem
{
    public class SkyLogic : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private readonly SkyView _skyView;

        public SkyLogic(LocalEvents localEvents, SkyView skyView)
        {
            _localEvents = localEvents;
            _skyView = skyView;

            _localEvents.OnDayTimeChange += ChangeSky;
        }

        private void ChangeSky(float dayTime)
        {
            _skyView.UpdateSkyColor(dayTime);
        }

        public void CleanUp()
        {
            _localEvents.OnDayTimeChange += ChangeSky;
        }
    }
}