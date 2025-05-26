using System;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Hero;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Utils;

namespace Scripts.Stat
{
    public class StatEffectLogic : ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;

        public StatEffectLogic(ProgressDataAdapter progressDataAdapter, LocalEvents localEvents)
        {
            _progressDataAdapter = progressDataAdapter;
            _localEvents = localEvents;

            _localEvents.OnActiveSprintByType += ChangeStatBySprintType;
        }

        private void ChangeStatBySprintType(SprintType sprintType)
        {
            switch (sprintType)
            {
                case SprintType.Dev:
                    DevProgressCallback();
                    break;
                case SprintType.Chill:
                    ChillProgressCallback();
                    break;
                default:
                    break;
            }
        }

        private void DevProgressCallback()
        {
            _progressDataAdapter.UpdateValue
                (Consts.Energy, _progressDataAdapter.GetMetadata(Consts.EnergySpendWhileWorking).Value);
            _progressDataAdapter.UpdateValue
                (Consts.Food, _progressDataAdapter.GetMetadata(Consts.FoodSpendWhileWorking).Value);
        }

        private void ChillProgressCallback()
        {
            _progressDataAdapter.UpdateValue(Consts.Energy, _progressDataAdapter.GetMetadata(Consts.EnergySpendWhileChilling).Value);
        }

        public void CleanUp()
        {
            _localEvents.OnActiveSprintByType -= ChangeStatBySprintType;
        }
    }
}