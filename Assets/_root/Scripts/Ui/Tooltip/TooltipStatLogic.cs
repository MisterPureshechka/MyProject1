using System.Collections.Generic;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Ui.TaskUi
{
    public class TooltipStatLogic : ICleanUp, IExecute
    {
        const float OFFSET = 50f;
        
        private readonly ProgressDataAdapterOLD _progressDataAdapterOld;
        private readonly TooltipView _tooltipView;
        private readonly PrefabDataBase _prefabData;
        private readonly LocalEvents _localEvents;
        private Canvas _canvas;
        private Vector2 _mousePos;
        
        private readonly List<TooltipStatItem> _activeItems = new();
        private MetaType _currentMetaType;
        private bool _isTooltipVisible;

        public TooltipStatLogic(ProgressDataAdapterOLD progressDataAdapterOld, TooltipView tooltipView, PrefabDataBase prefabData, LocalEvents localEvents, Canvas canvas)
        {
            _progressDataAdapterOld = progressDataAdapterOld;
            _tooltipView = tooltipView;
            _prefabData = prefabData;
            _localEvents = localEvents;
            _canvas = canvas;

            _localEvents.OnMouseEnterStat += ShowToolTip;         // только построить контент
            _localEvents.OnMouseMoveStat  += UpdateMousePos;      // только двигать
            _localEvents.OnMouseExitStat  += HideToolTip;    
        }
        
        
        public void ShowToolTip(MetaType metaType)
        {
            // if (_isTooltipVisible && _currentMetaType == metaType)
            //     return;
            //
            // HideToolTip();
            //
            // _currentMetaType = metaType;
            // _isTooltipVisible = true;
            // _tooltipView.gameObject.SetActive(true);
            //
            // foreach (var kvp in _progressDataAdapter.GetProgressData().Metadata)
            // {
            //     if (kvp.Value.MetaType != _currentMetaType) continue;
            //
            //     var statView = Object.Instantiate(_prefabData.TooltipItem, _tooltipView.StatHolder)
            //         .GetComponent<TooltipStatItem>();
            //     statView.SetInfo(kvp.Key, kvp.Value.Value);
            //     _activeItems.Add(statView);
            // }
            //
            // Canvas.ForceUpdateCanvases();
            // LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipView.StatHolder as RectTransform);
            //
            // UpdateTooltipPos();
        }

        public void HideToolTip()
        {
            foreach (Transform child in _tooltipView.StatHolder)
            {
                if(child) Object.Destroy(child.gameObject);
            }
            
            _activeItems.Clear();
            _isTooltipVisible = false;
            _tooltipView.gameObject.SetActive(false);
        }

        public void CleanUp()
        {
            _localEvents.OnMouseEnterStat -= ShowToolTip;         
            _localEvents.OnMouseMoveStat  -= UpdateMousePos;      
            _localEvents.OnMouseExitStat  -= HideToolTip;  
        }

        public void Execute(float deltatime)
        {
            UpdateTooltipPos();
        }
        
        private void UpdateMousePos(Vector2 position)
        {
            _mousePos = position;
            UpdateTooltipPos();
        }

        public void UpdateTooltipPos()
        {
            if (!_tooltipView.gameObject.activeSelf)
                return;

            Vector2 screenPos = _mousePos + new Vector2(OFFSET, 0);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _tooltipView.ToolTipRect.parent as RectTransform,
                screenPos,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out var localPoint
            );

            _tooltipView.ToolTipRect.localPosition = localPoint;
        }
    }
}