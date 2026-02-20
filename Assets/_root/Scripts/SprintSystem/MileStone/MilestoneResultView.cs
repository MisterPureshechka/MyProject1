using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tasks
{
    public class MilestoneResultView : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _root;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _revenue;
        [SerializeField] private TextMeshProUGUI _salary;
        [SerializeField] private TextMeshProUGUI _net;
        [SerializeField] private TextMeshProUGUI _total;
        [SerializeField] private TextMeshProUGUI _buttonText;

        [Header("Button")]
        [SerializeField] private Button _continueButton;

        public event Action OnContinue;

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => OnContinue?.Invoke());
            Hide();
        }

        public void Show(int revenue, int salary, int net, int total, string buttonText)
        {
            _root.SetActive(true);

            _title.text = "Milestone Complete";

            _revenue.text = $"Revenue\n<color=#4CAF50>+{revenue}$</color>";
            _salary.text  = $"Salary Expenses\n<color=#F44336>-{salary}$</color>";

            string netSign = net >= 0 ? "+" : "";
            string netColor = net >= 0 ? "#4CAF50" : "#F44336";

            _net.text = $"Net Profit\n<color={netColor}>{netSign}{net}$</color>";

            _total.text = $"Total Balance\n<color=#4CAF50>{total}$</color>";
            _buttonText.text = buttonText;
        }
        
        public void Hide()
        {
            _root.SetActive(false);
        }
    }
}