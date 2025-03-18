using System.Collections.Generic;
using Core;

    public class Controllers : IInitialization, IExecute, IFixedExecute, ICleanUp
    {
        private List<IInitialization> _initializeControllers;
        private List<IFixedExecute> _fixedExecuteControllers;
        private List<IExecute> _executeControllers;
        private List<ICleanUp> _cleanUpControllers;

        public Controllers()
        {
            _initializeControllers = new List<IInitialization>();
            _executeControllers = new List<IExecute>();
            _fixedExecuteControllers = new List<IFixedExecute>();
            _cleanUpControllers = new List<ICleanUp>();
        }

        public Controllers Add(IController controller)
        {
            if (controller is IInitialization initializeController)
            {
                _initializeControllers.Add(initializeController);
            }

            if (controller is IExecute executeController)
            {
                _executeControllers.Add(executeController);
            }

            if (controller is IFixedExecute lateExecuteController)
            {
                _fixedExecuteControllers.Add(lateExecuteController);
            }

            if (controller is ICleanUp cleanupController)
            {
                _cleanUpControllers.Add(cleanupController);
            }

            return this;
        }

        public void Initialization()
        {
            for (var index = 0; index < _initializeControllers.Count; ++index)
            {
                _initializeControllers[index].Initialize();
            }
        }

        public void Execute(float deltaTime)
        {
            for (var index = 0; index < _executeControllers.Count; ++index)
            {
                _executeControllers[index].Execute(deltaTime);
            }
        }
        

        public void FixedExecute(float fixedDeltaTime)
        {
            for (var index = 0; index < _fixedExecuteControllers.Count; ++index)
            {
                _fixedExecuteControllers[index].FixedExecute(fixedDeltaTime);
            }
        }

        public void CleanUp()
        {
            for (var index = 0; index < _cleanUpControllers.Count; ++index)
            {
                _cleanUpControllers[index].CleanUp();
            }
        }

        public void Initialize()
        {
            
        }
    }
