using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Ui;

namespace Scripts.Passion
{
    public class PassionLogic : IExecute, ICleanUp
    {
        private LocalEvents _localEvents;
        private ProgressDataAdapterOLD _progressDataAdapterOld;
        private SaveService _saveService;

        private IStatBarView _view;
        private float _timeSinceLastUpdate;
        private float _updateInterval = 0.5f;

        public PassionLogic(LocalEvents localEvents, ProgressDataAdapterOLD progressDataAdapterOld, SaveService saveService, IStatBarView view)
        {
            _localEvents = localEvents;
            _progressDataAdapterOld = progressDataAdapterOld;
            _saveService = saveService;
            _view = view;

            _localEvents.OnPassionIncrease += IncreasePassion;
            RegisterView(_view);
        }
        
        private void RegisterView(IStatBarView barView) 
        {
            _view = barView;
            _view.Init(_localEvents);
            var value = _progressDataAdapterOld.GetStats(barView.MetaType);
            var maxValue = _progressDataAdapterOld.GetMaxStats(barView.MetaType);
            _view.UpdateView(value, maxValue);
        }
        
        public void Execute(float deltaTime) 
        {
            _timeSinceLastUpdate += deltaTime;
            if (_timeSinceLastUpdate >= _updateInterval) 
            {
                var value = _progressDataAdapterOld.GetStats(_view.MetaType);
                var maxValue = _progressDataAdapterOld.GetMaxStats(_view.MetaType);

                _view.UpdateView(value, maxValue);
                _timeSinceLastUpdate = 0;
            }
        }

        private void IncreasePassion(PassionIncreaseType passionIncreaseType)
        {
            
        }

        private void DecreasePassion()
        {
            
        }
        
        public void CleanUp()
        {
            _localEvents.OnPassionIncrease -= IncreasePassion;
        }
    }
    
    public enum PassionIncreaseType
    {
        TaskComplete,
        SprintComplete,
    }
}