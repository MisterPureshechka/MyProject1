using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Tasks;
using Scripts.Ui;
using UnityEngine;

namespace Scripts.Progress
{
    public class ReleaseResultController : ICleanUp
    {
        private readonly ReleaseResultView _view;
        private readonly ProgressDataAdapter _progress;
        private readonly LocalEvents _events;
        private readonly GameStateMachine _stateMachine;

        public ReleaseResultController(
            ReleaseResultView view,
            ProgressDataAdapter progress,
            LocalEvents events,
            GameStateMachine stateMachine)
        {
            _view = view;
            _progress = progress;
            _events = events;
            _stateMachine = stateMachine;
                
            _events.OnReleaseWindow += Show;
            _view.OnContinue += OnContinue;
        }

        private void Show()
        {
            var result = _progress.Data.LastReleaseResult;

            if (!result.HasValue)
            {
                Debug.LogError("result is null");
                return;

            }
                
            Debug.LogError("Shown insignt controller");

            _view.Show(result, _progress.Data.Money);
        }
        
        private void OnContinue()
        {
            _view.Hide();
            _stateMachine.EnterState<ShopState>();
        }

        public void CleanUp()
        {
            Object.Destroy(_view.gameObject);
            _events.OnReleaseWindow -= Show;
            _view.OnContinue -= OnContinue;
        }
    }
}