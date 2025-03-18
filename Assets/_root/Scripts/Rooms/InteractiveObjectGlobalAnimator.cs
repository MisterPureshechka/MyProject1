using System.Collections.Generic;
using Core;
using Scripts.Data;

namespace Scripts.Rooms
{
    public class InteractiveObjectGlobalAnimator : IExecute, ICleanUp
    {
        private readonly InteractiveObjectConfig _config;
        private readonly InteractiveObjectRegisterer _registerer;
        private List<InteractiveObjectAnimator> _animators = new();

        public InteractiveObjectGlobalAnimator(InteractiveObjectConfig config, InteractiveObjectRegisterer registerer)
        {
            _config = config;
            _registerer = registerer;
            
            CreateAllAnimators(_registerer.GetInteractiveObjects());
        }

        private void CreateAllAnimators(List<IInteractiveObject> iOObjects)
        {
            foreach (var io in iOObjects)
            {
                var animator = new InteractiveObjectAnimator(_config, io);
                _animators.Add(animator);
            }
        }

        public void Execute(float deltatime)
        {
            foreach (var animator in _animators)
            {
                animator.Execute(deltatime);
            }
        }

        public void CleanUp()
        {
            _animators.Clear();
        }
    }
}