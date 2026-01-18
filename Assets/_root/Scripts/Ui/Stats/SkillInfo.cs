using TMPro;
using UnityEngine;

namespace _root.Scripts.Ui.Stats
{
    public class SkillInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _value;

        public void Set(string skillName, float value)
        {
            _name.text = skillName;
            _value.text = value.ToString("0.#");
        }
    }
}