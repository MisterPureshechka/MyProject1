using Core;
using Scripts.Meta;            
using Scripts.Progress;
using Scripts.Tasks;
using UnityEngine;             

namespace Scripts.Bugs
{
    public class BugLogic : ICleanUp
    {
        private readonly ProgressDataAdapterOLD _progress;

        private const float MinBugChance   = 0.05f;  // 5%
        private const float MaxBugChance   = 0.35f;  // 35%
        private const float MinBugSuccess  = 0.20f;  // 20%
        private const float MaxBugSuccess  = 0.80f;  // 80%

        private const float BugChancePow   = 1.2f;
        private const float BugSuccessPow  = 1.2f;

        private const int SuccessMin = 3;
        private const int SuccessMaxInclusive = 5;

        public BugLogic(ProgressDataAdapterOLD progress)
        {
            _progress = progress;
        }
        
        public float ComputeWellbeing()
        {
            float energyN = Normalize("Energy");
            float moodN   = Normalize("Mood");
            float hungerN = Normalize("Food");
            float toilet = Normalize("Toilet");
            
            float wellbeing = (energyN + moodN + hungerN + toilet) / 4f;
            return Mathf.Clamp01(wellbeing);
        }

        public float GetBugStartChance()
        {
            float w = ComputeWellbeing();            
            float t = Mathf.Pow(1f - w, BugChancePow); 
            return Mathf.Lerp(MinBugChance, MaxBugChance, t);
        }

        public float GetBugSuccessChance()
        {
            float w = ComputeWellbeing();
            float t = Mathf.Pow(w, BugSuccessPow);   
            return Mathf.Lerp(MinBugSuccess, MaxBugSuccess, t);
        }

        public bool TryRollBugStart(float maxProgress, out float progressToEmitBug)
        {
            float chance = GetBugStartChance();
            bool start = Random.value < chance;
            if (start)
            {
                progressToEmitBug = Random.Range(0f, maxProgress * 0.5f);
            }
            else
            {
                progressToEmitBug = -1f;
            }
            return start;
        }

        public void RollBugResult(out int value, out bool success)
        {
            float successChance = GetBugSuccessChance();
            success = Random.value < successChance;
            value = success ? Random.Range(SuccessMin, SuccessMaxInclusive + 1) : 1;
        }

        private float Normalize(string Metadata)
        {
            float cur = _progress.GetMetadata(Metadata).Value;
            float max = _progress.GetMetadata(Metadata).MaxValue;
            if (max <= 0.0001f) return 0f;
            return Mathf.Clamp01(cur / max);
        }

        public void CleanUp() {}
    }
}
