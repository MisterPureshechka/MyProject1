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
    }
}