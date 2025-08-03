using Scripts.GlobalStateMachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.OnlineShop
{
    public class OnlineShopButton : MonoBehaviour
    {
        [FormerlySerializedAs("_button")] [field: SerializeField] public Button Button;
        private LocalEvents _localEvents;

        public void Init(LocalEvents localEvents)
        {
            _localEvents = localEvents;
        }
    }
}