using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class ReadTaskButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button;
        [SerializeField] private Image _bookCover;
        [SerializeField] private TextMeshProUGUI _bookTitle;

        public void SetInfo(string bookTitle)
        {
            _bookTitle.text = bookTitle;
        }
    }
}