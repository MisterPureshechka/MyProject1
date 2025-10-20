using Core;
using Scripts.GlobalStateMachine;
using Scripts.Rooms;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.Hero
{
    public class CoffeeLogic : ICleanUp
    {
        private LocalEvents _localEvents;

        private float _coffeeValue;
        private float _coffeeMaxValue = 100f;
        private float _coffeeSpendValue = 0.001f;

        public CoffeeLogic(LocalEvents localEvents)
        {
            _localEvents = localEvents;

            _localEvents.OnTakeCoffee += TakeCoffee;
            _localEvents.OnActiveSprint += UpdateCoffeeValue;
            _localEvents.OnHeroGetIO += TakeCoffee;
        }

        private void TakeCoffee(InteractiveObjectType iO)
        {
            if (iO == InteractiveObjectType.CoffeeMachine)
            {
                _localEvents.TriggerCoffee(true);
            }
        }

        private void TakeCoffee(bool hasCoffee)
        {
            if (hasCoffee)
            {
                _coffeeValue = _coffeeMaxValue;
            }
        }

        private void UpdateCoffeeValue()
        {
            _coffeeValue -= _coffeeSpendValue;

            if (_coffeeValue <= 0)
            {
                Debug.Log("Coffee is over");
                _localEvents.TriggerCoffee(false);
            }
        }

        public void CleanUp()
        {
            _localEvents.OnTakeCoffee -= TakeCoffee;
            _localEvents.OnActiveSprint -= UpdateCoffeeValue;
            _localEvents.OnHeroGetIO -= TakeCoffee;
        }
    }
}