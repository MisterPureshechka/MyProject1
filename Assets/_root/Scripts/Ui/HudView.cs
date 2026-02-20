using Scripts.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private GameObject _timePanel;
        [SerializeField] private GameObject _daysLeftPanel;
        [SerializeField] private GameObject _stage;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _daysLeft;
        [SerializeField] private TextMeshProUGUI _experience;
        [SerializeField] private TextMeshProUGUI _money;
        [SerializeField] private TextMeshProUGUI _devStage;
        [field: SerializeField] public Button ReadyButton;

        public void SetWorkState(bool isWorkState, ProjectStage stage)
        {
            if (isWorkState)
            {
                ReadyButton.gameObject.SetActive(false);
                _timePanel.SetActive(true);
                _daysLeftPanel.SetActive(true);
                _stage.SetActive(true);
                SetStage(stage);
            }
            else
            {
                ReadyButton.gameObject.SetActive(true);
                _timePanel.SetActive(false);
                _daysLeftPanel.SetActive(false);
                _stage.SetActive(false);
            }
        }

        private void SetStage(ProjectStage stage)
        {
            _devStage.text = stage.ToString();
        }

        public void UpdateTime(int hours, int minutes)
        {
            _time.text = $"{hours:00}:{minutes:00}";
        }
        
        public void UpdateMoney(int money)
        {
            _money.text = money + "$";
        }

        public void UpdateExperience(int experience)
        {
            _experience.text = experience + "&";
        }

        public void UpdateDaysLeft(int daysLeft)
        {
            _daysLeft.text = daysLeft + " days left";
        }

        public void SetStageText(ProjectStage stage)
        {
            _devStage.text = stage.ToString();
        }
    }
}