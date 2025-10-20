using Scripts.Ui;
using TMPro;
using UnityEngine;

namespace Scripts.GameDev
{
    public class ProgressInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private BaseTextAnimation _textAnimation;
        
        private int _total = -1;

        public void InitTitle(string title)
        {
            _titleText.text = title;
        }

        public void SetValue(int completed)
        {
            if (_total > 0)
            {
                _valueText.text = $"{completed}";
            }
            else
            {
                _valueText.text = completed.ToString();
            }
        }

        public void AniamteText()
        {
            _textAnimation.AnimateText();
        }
    }
}