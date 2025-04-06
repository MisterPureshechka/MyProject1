using System;
using Core;
using Scripts.Hero;
using Scripts.Progress;
using Scripts.Utils;

namespace Scripts.Stat
{
    public class StatEffectLogic : ICleanUp
    {
        private readonly HeroLogic _heroLogic;
        private readonly ProgressDataAdapter _progressDataAdapter;

        public StatEffectLogic(HeroLogic heroLogic, ProgressDataAdapter progressDataAdapter)
        {
            _heroLogic = heroLogic;
            _progressDataAdapter = progressDataAdapter;

            _heroLogic.OnDevActiveStat += DevProgressCallback;
            _heroLogic.OnChillActiveStat += ChillProgressCallback;
        }

        private void DevProgressCallback()
        {
            _progressDataAdapter.UpdateValue(Consts.Energy, _progressDataAdapter.GetMetadata(Consts.EnergySpendWhileWorking).Value);
        }

        private void ChillProgressCallback()
        {
            _progressDataAdapter.UpdateValue(Consts.Energy, _progressDataAdapter.GetMetadata(Consts.EnergySpendWhileChilling).Value);
        }

        public void CleanUp()
        {
            _heroLogic.OnDevActiveStat -= DevProgressCallback;
            _heroLogic.OnChillActiveStat -= ChillProgressCallback;
        }
    }
}