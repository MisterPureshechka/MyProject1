using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "ScriptableObjects/HeroConfig", order = 1)]
    public class HeroConfig : ScriptableObject
    {
        [field: SerializeField] public float WalkSpeed { get; private set; }   
    }
}