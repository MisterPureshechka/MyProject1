using System;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Hero;
using Scripts.Progress;
using Scripts.Tasks;
using Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Scripts.Stat
{
    public class StatEffectLogic : ICleanUp
    {
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly LocalEvents _localEvents;
        private readonly TextMeshProUGUI _tempStat;

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
                case SprintType.Eat:
                    EatProgressCallBack();
                    break;
                default:
                    break;
            }
        }

        private void EatProgressCallBack()
        {
            _progressDataAdapter.UpdateValue
                (Consts.Energy, _progressDataAdapter.GetMetadata(Consts.EnergyOnEating).Value);
            _progressDataAdapter.UpdateValue
                (Consts.Food, _progressDataAdapter.GetMetadata(Consts.FoodOnEating).Value);
            
            _progressDataAdapter.UpdateValue
                (Consts.Shower, _progressDataAdapter.GetMetadata(Consts.ShowerOnEating).Value);
        }

        public void ShowerProgressCallback()
        {
            _progressDataAdapter.UpdateValue
                (Consts.Shower, _progressDataAdapter.GetMetadata(Consts.ShowerOnShower).Value);
        }

        private void DevProgressCallback()
        {
            _progressDataAdapter.UpdateValue
                (Consts.Energy, _progressDataAdapter.GetMetadata(Consts.EnergySpendWhileWorking).Value);
            _progressDataAdapter.UpdateValue
                (Consts.Food, _progressDataAdapter.GetMetadata(Consts.FoodSpendWhileWorking).Value);
            _progressDataAdapter.UpdateValue
                (Consts.Shower, _progressDataAdapter.GetMetadata(Consts.ShowerOnWorking).Value);
            _progressDataAdapter.UpdateValue
                (Consts.Mood, _progressDataAdapter.GetMetadata(Consts.MoodOnWorking).Value);
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