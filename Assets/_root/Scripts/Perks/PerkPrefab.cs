using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Perks
{
    public class PerkPrefab : MonoBehaviour
    {
        [field: SerializeField] public Button Button;
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _perkName;
        [SerializeField] private Image _perkImage;

        public void SetInfo(PerkData perkData, Sprite perkSprite)
        {
            _perkName.text = perkData.Name;
            _perkImage.sprite = perkSprite;
        }

        public void Show()
        {
            
        }

        public void SetSelected(bool nowSelected)
        {
            if (nowSelected)
            {
                _root.DOMoveY(0.1f, 0.5f).SetEase(Ease.OutBounce);
            }
            else
            {
                _root.DOMoveY(0f, 0.5f).SetEase(Ease.OutBounce);
            }
            
        }
    }
}