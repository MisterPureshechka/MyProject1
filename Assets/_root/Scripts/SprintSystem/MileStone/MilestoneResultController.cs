using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Tasks
{
    public class MilestoneResultController : ICleanUp
    {
        private readonly MilestoneResultView _view;
        private readonly ProgressDataAdapter _progress;
        private readonly GameStateMachine _stateMachine;
        private readonly LocalEvents _events;
        private readonly SaveService _saveService;

        public MilestoneResultController(
            MilestoneResultView view,
            ProgressDataAdapter progress,
            GameStateMachine stateMachine,
            LocalEvents events,
            SaveService saveService)
        {
            _view = view;
            _progress = progress;
            _stateMachine = stateMachine;
            _events = events;
            _saveService = saveService;

            _events.OnMilestoneResultWindow += Show;
            _view.OnContinue += OnContinue;
        }

        private void Show()
        {
            var result = _progress.Data.LastMilestoneResult;
            var releaseNow = _progress.Data.PendingReleaseWindow;

            _view.Show(
                result.MoneyReward,
                result.SalaryCost,
                result.NetProfit,
                result.MoneyTotalAfter,
                releaseNow ? "Release Now" : "Continue"
            );
        }

        private void OnContinue()
        {
            _view.Hide();

            var data = _progress.Data;

            if (data.PendingReleaseWindow)
            {
                data.PendingReleaseWindow = false;
                _saveService.SaveProgress(data);

                _events.TriggerReleaseWindow();
                return;
            }

            _stateMachine.EnterState<ShopState>();
        }

        public void CleanUp()
        {
            Object.Destroy(_view.gameObject);
            _events.OnMilestoneResultWindow -= Show;
            _view.OnContinue -= OnContinue;
        }
    }
}
