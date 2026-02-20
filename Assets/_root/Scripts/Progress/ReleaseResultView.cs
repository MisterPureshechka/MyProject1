using System;
using Scripts.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class ReleaseResultView : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _root;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _scores;
        [SerializeField] private TextMeshProUGUI _sales;
        [SerializeField] private TextMeshProUGUI _revenue;
        [SerializeField] private TextMeshProUGUI _publisher;
        [SerializeField] private TextMeshProUGUI _net;
        [SerializeField] private TextMeshProUGUI _awards;

        [Header("Button")]
        [SerializeField] private Button _continueButton;

        public event Action OnContinue;

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => OnContinue?.Invoke());
            Hide();
        }

        public void Show(ReleaseResultData result, int totalMoneyAfter)
        {
            Debug.LogError("Shown");
            _root.SetActive(true);

            _title.text = $"Game #{result.GameIndex + 1} Released!";

            _scores.text =
                $"Art: {result.ArtScore}\n" +
                $"Gameplay: {result.GameplayScore}\n" +
                $"Sound: {result.SoundScore}";

            _sales.text = $"Units Sold\n{result.UnitsSold:N0}";

            _revenue.text = $"Revenue\n+{result.Revenue}$";

            _publisher.text = result.PublisherCut > 0
                ? $"Publisher Cut\n-{result.PublisherCut}$"
                : "Independent Release";

            string netSign = result.NetProfit >= 0 ? "+" : "";
            string netColor = result.NetProfit >= 0 ? "#4CAF50" : "#F44336";

            _net.text =
                $"Net Profit\n<color={netColor}>{netSign}{result.NetProfit}$</color>";

            _awards.text = BuildAwardsText(result);

            // можно добавить строку с балансом
            // $"Total Balance\n{totalMoneyAfter}$"
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        private string BuildAwardsText(ReleaseResultData r)
        {
            string text = "Awards:\n";

            if (r.AwardArt) text += "🏆 Best Art\n";
            if (r.AwardGameplay) text += "🏆 Best Gameplay\n";
            if (r.AwardSound) text += "🏆 Best Sound\n";
            if (r.GameOfTheYear) text += "🌟 Game of the Year!\n";

            if (!r.AwardArt && !r.AwardGameplay && !r.AwardSound && !r.GameOfTheYear)
                text += "No major awards this time.";

            return text;
        }
    }
}
