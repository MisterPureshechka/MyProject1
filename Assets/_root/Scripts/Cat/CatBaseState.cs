using Scripts.GlobalStateMachine;
using UnityEngine;

namespace Scripts.Cat
{
    public class CatBaseState : IState
    {
        protected readonly CatLogic _catLogic;

        public CatBaseState(CatLogic catLogic)
        {
            _catLogic = catLogic;
        }
        
        public virtual void Enter()
        {
        }

        public virtual void Update(float deltaTime)
        {
            //Debug.Log("Change state to " + this.ToString());
        }

        public virtual void Exit()
        {
        }
    }
}