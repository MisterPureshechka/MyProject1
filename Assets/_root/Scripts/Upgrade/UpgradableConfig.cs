using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Upgrade
{
    [CreateAssetMenu(fileName = "Upgradables", menuName = "ScriptableObjects/Upgradables")]
    public class UpgradableConfig : ScriptableObject
    {
        [field: SerializeField] public List<UpgradableData> Upgradables;
    }
}