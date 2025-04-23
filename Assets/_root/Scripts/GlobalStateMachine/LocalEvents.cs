using System;
using Core;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.GlobalStateMachine
{
    public class LocalEvents : IController
    {
        public Action OnClosePanel { get; set; }
        public void TriggerClosePanel() => OnClosePanel?.Invoke();
        
        public Action OnOpenPanel { get; set; }
        public void TriggerOpenPanel() => OnOpenPanel?.Invoke();

        public Action<SprintType> OnHeroGetIO;
        public void TriggerHeroGetIO(SprintType iOType) => OnHeroGetIO?.Invoke(iOType);
        
        public Action<Vector2> OnMouseClickWorld {get; set;}
        public void TriggerMouseClickedWorld(Vector2 pos) => OnMouseClickWorld?.Invoke(pos);

        public Action<SprintType, Vector2> OnMouseClickIO { get; set; }
        public void TriggerMouseClickedIO(SprintType iOType, Vector2 pos) => OnMouseClickIO?.Invoke(iOType, pos);
        
        public Action<Vector2> OnMouseClickUI;
        public void TriggerMouseClickedUI(Vector2 pos) => OnMouseClickUI?.Invoke(pos);
        
        public Action<Vector2> OnMousePositionChange;
        public void TriggerMousePositionChange(Vector2 pos) => OnMousePositionChange?.Invoke(pos);

        public Action<bool> OnGetSupportedType { get; set; }
        public void GetSupportedTypeResult(bool isSupportedType) => OnGetSupportedType?.Invoke(isSupportedType);
        
        public Action OnTasksApply { get; set; }
        public void TriggerTasksApply() => OnTasksApply?.Invoke();
        
        public Action<Vector3> OnGetHeroPos { get; set; }
        public void TriggerGetHeroPos(Vector3 pos) => OnGetHeroPos?.Invoke(pos);

        public Action OnClickEmpty {get; set;}
        public void TriggerEmptyClick() => OnClickEmpty?.Invoke();

        public Action OnAllTaskShow { get; set; }
        public void TriggerAllTaskShow() => OnAllTaskShow?.Invoke();

        public Action OnAllTaskHide { get; set; }
        public void TriggerAllTaskHide() => OnAllTaskHide?.Invoke();
        
        public Action<SprintType> OnSprintCreated {get; set;}
        public void TriggerSprintCreated(SprintType type) => OnSprintCreated?.Invoke(type);

        public Action<SprintType> OnSprintContinue { get; set; }
        public void TriggerSprintContinue(SprintType sprintType) => OnSprintContinue?.Invoke(sprintType);

        public Action<bool, SprintType> OnActiveState { get; set; }
        public void TriggerActiveState(bool isActive, SprintType sprintType) => OnActiveState?.Invoke(isActive, sprintType);

        public Action OnDevActiveState { get; set; }
        public void TriggerDevActiveState() => OnDevActiveState?.Invoke();
        
        public Action<SprintType> OnActiveSprintByType { get; set; }
        public void TriggerActiveSprintByType(SprintType sprintType) => OnActiveSprintByType?.Invoke(sprintType);
        
        public Action OnSprintExit { get; set; }
        public void TriggerSprintExit() => OnSprintExit?.Invoke();
        
        public Action<SprintType> OnSprintClosed { get; set; }
        public void TriggerSprintClosed(SprintType sprintType) => OnSprintClosed?.Invoke(sprintType);

        public Action<SprintType> OnWalkToIO { get; set; }
        public void TriggerWalkToIO(SprintType sprintType) => OnWalkToIO?.Invoke(sprintType);

        public Action<SprintType> OnAutoSprintCreated { get; set; }
        public void TriggerCreateAutoSprint(SprintType type) => OnAutoSprintCreated?.Invoke(type);
    }
}