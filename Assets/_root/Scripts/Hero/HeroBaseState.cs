using Scripts.Data;
using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Hero
{
    public abstract class HeroBaseState : IState
    {
        protected HeroLogic _heroLogic;

        protected HeroBaseState(HeroLogic heroLogic)
        {
            _heroLogic = heroLogic;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update(float deltaTime)
        {  
            Debug.Log(GetType());
        }

        public virtual void Exit()
        {
        }
    }
}