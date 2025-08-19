using UnityEngine;

namespace Scripts.Messenger
{
    [CreateAssetMenu(menuName = "MessengerConfig", fileName = "MessengerConfig")]
    public class MessengerConfig : ScriptableObject
    {
        [field: SerializeField] public float JobChance = 0.1f;
    }
}