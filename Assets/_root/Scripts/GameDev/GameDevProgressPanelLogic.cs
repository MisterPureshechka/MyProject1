using Core;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;

namespace Scripts.GameDev
{
    public class GameDevProgressPanelLogic : ICleanUp
    {
        private GameDevProgressPanel _gameDevProgressPanel;
        private GameDevProgress _gameDevProgress;
        private TaskLibrary _taskLibrary;
        private readonly LocalEvents _localEvents;

        public GameDevProgressPanelLogic(GameDevProgressPanel gameDevProgressPanel, GameDevProgress gameDevProgress, TaskLibrary taskLibrary, LocalEvents localEvents)
        {
            _gameDevProgressPanel = gameDevProgressPanel;
            _gameDevProgress = gameDevProgress;
            _taskLibrary = taskLibrary;
            _localEvents = localEvents;

            _gameDevProgressPanel.Init(_gameDevProgress, "New Game", _taskLibrary);
            _gameDevProgressPanel.Rebuild();

            _localEvents.OnDevTaskComplete += RefreshValues;
        }

        private void RefreshValues(DevTaskType devTaskType)
        {
            _gameDevProgressPanel.RefreshValues(devTaskType);
        }

        public void CleanUp()
        {
            _localEvents.OnDevTaskComplete -= RefreshValues;
        }
    }
}