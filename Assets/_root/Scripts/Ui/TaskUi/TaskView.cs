using TMPro;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class TaskView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _progressInfo;

        public void SetInfo(string titleText, float progressText)
        {
            _titleText.text = titleText;
            _progressInfo.text = progressText.ToString();
        }

        public void UpdateProgress(float progress)
        {
            _progressInfo.text = progress.ToString("P");
        }
    }
}