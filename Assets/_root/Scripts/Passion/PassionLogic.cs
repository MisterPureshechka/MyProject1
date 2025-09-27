using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Ui;

namespace Scripts.Passion
{
    public class PassionLogic : IExecute, ICleanUp
    {
        private LocalEvents _localEvents;
        private ProgressDataAdapter _progressDataAdapter;
        private GameProgress _gameProgress;

        private IStatBarView _view;
        private float _timeSinceLastUpdate;
        private float _updateInterval = 0.5f;

        public PassionLogic(LocalEvents localEvents, ProgressDataAdapter progressDataAdapter, GameProgress gameProgress, IStatBarView view)
        {
            _localEvents = localEvents;
            _progressDataAdapter = progressDataAdapter;
            _gameProgress = gameProgress;
            _view = view;

            _localEvents.OnPassionIncrease += IncreasePassion;
            RegisterView(_view);
        }
        
        private void RegisterView(IStatBarView barView) 
        {
            _view = barView;
            _view.Init(_localEvents);
            var value = _progressDataAdapter.GetStats(barView.MetaType);
            var maxValue = _progressDataAdapter.GetMaxStats(barView.MetaType);
            _view.UpdateView(value, maxValue);
        }
        
        public void Execute(float deltaTime) 
        {
            _timeSinceLastUpdate += deltaTime;
            if (_timeSinceLastUpdate >= _updateInterval) 
            {
                var value = _progressDataAdapter.GetStats(_view.MetaType);
                var maxValue = _progressDataAdapter.GetMaxStats(_view.MetaType);

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