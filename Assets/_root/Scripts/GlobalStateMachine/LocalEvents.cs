using System;
using Core;
using Scripts.Meta;
using Scripts.Rooms;
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

        public Action<SprintType> OnHeroGetIO { get; set; }
        public void TriggerHeroGetIO(SprintType iOType) => OnHeroGetIO?.Invoke(iOType);
        
        public Action<Vector2> OnMouseClickWorld {get; set;}
        public void TriggerMouseClickedWorld(Vector2 pos) => OnMouseClickWorld?.Invoke(pos);

        public Action<InteractiveObjectType, Vector2> OnMouseClickIO { get; set; }
        public void TriggerMouseClickedIO(InteractiveObjectType iOType, Vector2 pos) => OnMouseClickIO?.Invoke(iOType, pos);
        
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

        public Action<SprintType> OnTaskCatalogShow { get; set; }
        public void TriggerAllTaskShow(SprintType type) => OnTaskCatalogShow?.Invoke(type);

        public Action<SprintType> OnTaskCatalogHide { get; set; }
        public void TriggerTaskCatalogHide(SprintType type) => OnTaskCatalogHide?.Invoke(type);
        
        public Action<SprintType> OnSprintCreated {get; set;}
        public void TriggerSprintCreated(SprintType type) => OnSprintCreated?.Invoke(type);

        public Action<SprintType> OnSprintContinue { get; set; }
        public void TriggerSprintContinue(SprintType sprintType) => OnSprintContinue?.Invoke(sprintType);

        public Action<bool, SprintType> OnActiveState { get; set; }
        public void TriggerActiveState(bool isActive, SprintType sprintType) => OnActiveState?.Invoke(isActive, sprintType);

        public Action OnActiveSprint { get; set; }
        
        public Action<SprintType> OnActiveSprintByType { get; set; }

        public void TriggerActiveSprintByType(SprintType sprintType)
        {
            OnActiveSprintByType?.Invoke(sprintType);
            OnActiveSprint?.Invoke();
        }
        
        public Action OnSprintExit { get; set; }
        public void TriggerSprintExit() => OnSprintExit?.Invoke();
        
        public Action<SprintType> OnSprintClosed { get; set; }
        public void TriggerSprintClosed(SprintType sprintType) => OnSprintClosed?.Invoke(sprintType);

        public Action<SprintType> OnWalkToIO { get; set; }
        public void TriggerWalkToIO(SprintType sprintType) => OnWalkToIO?.Invoke(sprintType);
        
        public Action OnHeroWalkToIO { get; set; }
        public void TriggerHeroWalkToIO() => OnHeroWalkToIO?.Invoke();

        public Action<SprintType> OnAutoSprintCreated { get; set; }
        public void TriggerCreateAutoSprint(SprintType type) => OnAutoSprintCreated?.Invoke(type);

        public Action<SprintType> OnSprintComplete { get; set; }
        public void TriggerSprintComplete(SprintType sprintType) => OnSprintComplete?.Invoke(sprintType);

        public Action<SprintType> OnHeroGetRootIO { get; set; }
        public void TriggerHeroGetRootIO(SprintType targetIOSprintType) => OnHeroGetRootIO?.Invoke(targetIOSprintType);

        public Action<bool> OnMouseOverSideRoom { get; set; }
        public void TriggerMouseOverSideRoom(bool sideRoomIsLeftRoom) => OnMouseOverSideRoom?.Invoke(sideRoomIsLeftRoom);
        
        public Action OnMouseOverMainRoom;
        public void TriggerMouseOverMainRoom() => OnMouseOverMainRoom?.Invoke();

        public Action OnMouseOverKitchen;
        public void TriggerMouseOverKitchen() => OnMouseOverKitchen?.Invoke();

        public Action OnMouseOverToilet;
        public void TriggerMouseOverToilet() => OnMouseOverToilet?.Invoke();
        public Action<MetaType, Vector2> OnMouseOverStat;
        public void TriggerMouseOverStat(MetaType metaType, Vector2 pos) => OnMouseOverStat?.Invoke(metaType, pos);

        public Action OnMouseExitStat;
        public void TriggerMouseExitStat() => OnMouseExitStat?.Invoke();

        public Action<DevTaskType> OnReadTaskUpdate;
        public void TriggerReadTaskUpdate(DevTaskType knowledgeToUpgrade) => OnReadTaskUpdate?.Invoke(knowledgeToUpgrade);

        public Action<float> OnDayTimeChange;
        public void TriggerOnDayTimeChange(float time) => OnDayTimeChange?.Invoke(time);

        public Action<float> OnNormilizeDayTimeChange;
        public void TriggerNormalizeDayTimeChange(float time) => OnNormilizeDayTimeChange?.Invoke(time);
        
        public Action<float> OnNormalizeNightTimeChange;
        public void TriggerNormalizeNightTimeChange(float time) => OnNormalizeNightTimeChange?.Invoke(time);

        public Action OnNewDay;
        public void TriggerNewDay() => OnNewDay?.Invoke();

        public Action OnMiniCalendarButtonClick;
        public void TriggerMiniCalendarButtonOpen() => OnMiniCalendarButtonClick?.Invoke();
    }
}