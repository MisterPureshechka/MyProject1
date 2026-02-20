using System;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Tasks;

namespace Scripts.Ui
{
    public class HudController : ICleanUp
    {
        private HudView _hudView;
        private readonly TimeService _time;
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;
        private readonly GameStateMachine _stateMachine;

        private event Action _onTimeUpdated;
        private event Action<int> _onMoneyUpdated;
        private event Action<int> _onExperienceUpdated;
        
        private ProjectStage _currentStage;

        public HudController(HudView hudView, TimeService time, Action<int> onMoneyUpdated, Action<int> onExperienceUpdated, ProgressDataAdapter progressDataAdapter, LocalEvents localEvents, bool isWorkState, GameStateMachine stateMachine)
        {
            _hudView = hudView;
            _time = time;
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;
            _stateMachine = stateMachine;

            _onMoneyUpdated = onMoneyUpdated;
            _onExperienceUpdated = onExperienceUpdated;

            _currentStage = _progressDataAdapter.Data.Stage;

            _hudView.SetWorkState(isWorkState, _currentStage);
            _hudView.ReadyButton.onClick.AddListener(ReadyButtonListener);

            _localEvents.OnMilestoneProgressChanged += LoadInfo;
            _localEvents.OnTimeUpdated += OnTimeUpdated;
            _localEvents.OnTaskComplete += ExperienceUpdateListener;
            _localEvents.OnWalletUpdate += UpdateMoney;

            LoadInfo();
            
            
        }

        private void ReadyButtonListener()
        {
            _stateMachine.EnterState<WorkState>();
        }


        private void LoadInfo()
        {
            var data = _progressDataAdapter.Data;

            if (_currentStage != data.Stage)
            {
                _currentStage = data.Stage;
                _hudView.SetStageText(_currentStage);
            }

            var mp = data.MilestoneProgress;
            var daysLeft = mp.DaysLimit - mp.DaysSpent;
            UpdateDaysLeft(daysLeft);

            UpdateMoney(data.Money);
            UpdateExperience(data.Experience);
        }

        public void UpdateMoney(int money)
        {
            _hudView.UpdateMoney(money);
        }

        public void UpdateExperience(int experience)
        {
            _hudView.UpdateExperience(experience);
        }

        private void ExperienceUpdateListener()
        {
            var data = _progressDataAdapter.Data;
            var experience = data.Experience;
            UpdateExperience(experience);
        }

        public void UpdateDaysLeft(int daysLeft)
        {
            _hudView.UpdateDaysLeft(daysLeft);
        }

        private void OnTimeUpdated(float day01)
        {
            TimeUtils.NormalizedToClock(day01, out var h, out var m);
            _hudView.UpdateTime(h, m);
        }

        public void CleanUp()
        {
            _localEvents.OnMilestoneProgressChanged -= LoadInfo;
            _localEvents.OnTimeUpdated -= OnTimeUpdated;
            _localEvents.OnTaskComplete -= ExperienceUpdateListener;
            _localEvents.OnWalletUpdate -= UpdateMoney;
            _hudView.ReadyButton.onClick.RemoveAllListeners(); 
            UnityEngine.Object.Destroy(_hudView.gameObject);
        }
    }
}