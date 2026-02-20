using _root.Scripts.Ui.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.SkillUpgrade
{
    public class SkillShopItem : MonoBehaviour
    {
        public Button BuyButton;
        
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private Transform _skillContainer;
        [SerializeField] private Transform _previewContainer;
        [SerializeField] private SkillInfo _skillInfoPrefab;
        
        public SkillUpgradeOffer SkillOffer { get; private set; }

        public void Init(SkillUpgradeOffer offer)
        {
            SkillOffer = offer;
            foreach (var skill in offer.SkillUpgradeMap.Keys)
            {
                if (offer.SkillUpgradeMap[skill] <= 0f)
                    continue;
                
                var skillInstance = Instantiate(_skillInfoPrefab, _skillContainer); 
                skillInstance.Set(skill.ToString(), offer.SkillUpgradeMap[skill]);
            }
            _price.text = SkillOffer.SkillUpgradeCost + "&";
        }

        public void Destroy()
        {
            BuyButton.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}