using TMPro;
using UnityEngine;

namespace Scripts.Ui
{
    public class TooltipUI : MonoBehaviour
    {
        public static TooltipUI Instance;

        [SerializeField] private GameObject _tooltipRoot;
        [SerializeField] private TextMeshProUGUI _tooltipText;
        [SerializeField] private RectTransform _background;

        private void Awake()
        {
            Instance = this;
            Hide();
        }

        public void Show(string text, Vector3 worldPosition)
        {
            _tooltipText.text = text;
            _tooltipRoot.SetActive(true);
            
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            _tooltipRoot.transform.position = screenPos;
        }

        public void Hide()
        {
            _tooltipRoot.SetActive(false);
        }
    }
}