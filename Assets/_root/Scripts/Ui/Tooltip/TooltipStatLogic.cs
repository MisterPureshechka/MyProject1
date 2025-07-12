using System.Collections.Generic;
using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Ui.TaskUi
{
    public class TooltipStatLogic : ICleanUp, IExecute
    {
        const float OFFSET = 50f;
        
        private readonly ProgressDataAdapter _progressDataAdapter;
        private readonly TooltipView _tooltipView;
        private readonly PrefabDataBase _prefabData;
        private readonly LocalEvents _localEvents;
        private Canvas _canvas;
        private Vector2 _mousePos;
        
        private readonly List<TooltipStatItem> _activeItems = new();
        private MetaType _currentMetaType;
        private bool _isTooltipVisible;

        public TooltipStatLogic(ProgressDataAdapter progressDataAdapter, TooltipView tooltipView, PrefabDataBase prefabData, LocalEvents localEvents, Canvas canvas)
        {
            _progressDataAdapter = progressDataAdapter;
            _tooltipView = tooltipView;
            _prefabData = prefabData;
            _localEvents = localEvents;
            _canvas = canvas;

            _localEvents.OnMouseOverStat += ShowToolTip;
            _localEvents.OnMouseExitStat += HideToolTip;
        }
        public void ShowToolTip(MetaType metaType, Vector2 position)
        {
            HideToolTip(); // очищает старые

            _currentMetaType = metaType;
            _isTooltipVisible = true;
            _mousePos = position;

            _tooltipView.gameObject.SetActive(true);

            foreach (var kvp in _progressDataAdapter.GetProgressData().Metadata)
            {
                if (kvp.Value.MetaType != _currentMetaType)
                    continue;

                var statView = Object.Instantiate(_prefabData.TooltipItem, _tooltipView.StatHolder).GetComponent<TooltipStatItem>();
                statView.SetInfo(kvp.Key, kvp.Value.Value);
                _activeItems.Add(statView); // сохраняем
            }
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
            _localEvents.OnMouseOverStat -= ShowToolTip;
        }

        public void Execute(float deltatime)
        {
            UpdateTooltipInfo();
            UpdateTooltipPos();
        }

        private void UpdateTooltipInfo()
        {
            if (!_isTooltipVisible)
                return;
            
            int i = 0;
            foreach (var kvp in _progressDataAdapter.GetProgressData().Metadata)
            {
                if (kvp.Value.MetaType != _currentMetaType)
                    continue;

                if (i < _activeItems.Count)
                {
                    _activeItems[i].SetInfo(kvp.Key, kvp.Value.Value);
                    i++;
                }
            }
        }

        public void UpdateTooltipPos()
        {
            if (!_tooltipView.gameObject.activeSelf)
                return;
            
            _tooltipView.ToolTipRect.position = _mousePos + new Vector2(OFFSET, 0);
        }
    }
}