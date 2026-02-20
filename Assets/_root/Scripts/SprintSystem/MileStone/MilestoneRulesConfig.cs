using UnityEngine;

namespace Scripts.Tasks
{
    [CreateAssetMenu(menuName = "Configs/MilestoneRulesConfig")]
    public class MilestoneRulesConfig : ScriptableObject
    {
        [Header("Task distribution")]
        [SerializeField] private float _skillToWeight = 0.2f;
        public float SkillToWeight => _skillToWeight;

        // -------------------------
        // BASE WEIGHTS
        // -------------------------

        public float GetBaseWeight(DevTaskType type)
        {
            switch (type)
            {
                case DevTaskType.Programming: return 1.0f;
                case DevTaskType.Art:         return 1.0f;
                case DevTaskType.SoundDesign: return 1.0f;
                case DevTaskType.GameDesign:  return 1.0f;
                case DevTaskType.Marketing:   return 1.0f;
                default:                      return 1.0f;
            }
        }

        public int GetMinTasks(DevTaskType type, int milestoneIndex) => 0;

        // -------------------------
        // BASE TASK COUNT (у тебя уже ок)
        // -------------------------

        public int GetTaskCount(int gameIndex, int milestoneIndex)
        {
            int baseCount = 1 + milestoneIndex * 2;  // майлстоун глубже -> больше задач
            int growth    = gameIndex * 1;           // игра дальше -> больше задач
            return Mathf.Max(1, baseCount + growth);
        }

        // -------------------------
        // BASE DAYS LIMIT (логика)
        // -------------------------
        // Базовые дни ДО авто-тюнинга от teamSkillScore.
        // Можно сделать так: с прогрессом игр дедлайны сжимаются,
        // а с ростом milestoneIndex чуть расширяются (или наоборот — как тебе надо).

        [Header("Base days formula")]
        [SerializeField] private int _baseDays = 6;
        [SerializeField] private int _daysPerMilestone = 1;     // +дни за milestoneIndex
        [SerializeField] private float _daysPerGame = -0.3f;    // -дни за gameIndex (сжатие дедлайна)

        public int GetDaysLimit(int gameIndex, int milestoneIndex)
        {
            float days =
                _baseDays +
                milestoneIndex * _daysPerMilestone +
                gameIndex * _daysPerGame;

            return Mathf.Max(1, Mathf.RoundToInt(days));
        }

        // -------------------------
        // BASE MONEY REWARD (логика)
        // -------------------------
        // Базовая награда, которая растет от количества задач и прогресса по играм.
        // Дальше на закрытии майлстоуна умножишь через ComputeMoneyReward на completion.

        [Header("Base reward formula")]
        [SerializeField] private int _rewardBase = 50;
        [SerializeField] private int _rewardPerTask = 25;
        [SerializeField] private int _rewardPerGame = 40;
        [SerializeField] private int _rewardPerMilestone = 20;

        public int GetMoneyReward(int gameIndex, int milestoneIndex)
        {
            int tasks = GetTaskCount(gameIndex, milestoneIndex);

            int reward =
                _rewardBase +
                tasks * _rewardPerTask +
                gameIndex * _rewardPerGame +
                milestoneIndex * _rewardPerMilestone;

            return Mathf.Max(0, reward);
        }

        // -------------------------
        // AUTO DAYS (корректировка от teamSkillScore)
        // -------------------------

        [Header("Days auto-tuning")]
        [SerializeField] private bool _useAutoDays = true;
        [SerializeField] private float _daysPerReleasedGame = 0.08f;
        [SerializeField] private float _skillToDaysK = 0.02f;
        [SerializeField] private float _minDaysMultiplier = 0.60f;
        [SerializeField] private float _maxDaysMultiplier = 1.20f;

        public int ComputeDaysLimit(int baseDays, int gameIndex, float teamSkillScore)
        {
            if (!_useAutoDays)
                return baseDays;

            float gameDifficulty = 1f + gameIndex * _daysPerReleasedGame;

            // как ты хотел: меньше скилл -> меньше дней (жестче)
            float skillMultiplier = Mathf.Clamp(
                _minDaysMultiplier + teamSkillScore * _skillToDaysK,
                _minDaysMultiplier,
                _maxDaysMultiplier);

            float days = baseDays * gameDifficulty * skillMultiplier;
            return Mathf.Max(1, Mathf.RoundToInt(days));
        }

        // -------------------------
        // AUTO REWARD (по выполненным таскам)
        // -------------------------

        [Header("Reward auto-tuning")]
        [SerializeField] private bool _useAutoReward = true;
        [SerializeField] private float _completionPower = 1.6f; // мотивация добивать
        [SerializeField] private float _minPartialFactor = 0.10f;

        public int ComputeMoneyReward(int baseReward, int plannedTasks, int completedTasks)
        {
            if (!_useAutoReward)
                return baseReward;

            float completion = plannedTasks <= 0 ? 0f : (float)completedTasks / plannedTasks;
            completion = Mathf.Clamp01(completion);

            float factor = Mathf.Pow(completion, _completionPower);

            float reward = baseReward * factor;

            // минималка за частичное выполнение
            reward = Mathf.Max(reward, baseReward * _minPartialFactor * completion);

            return Mathf.RoundToInt(reward);
        }
    }
}
