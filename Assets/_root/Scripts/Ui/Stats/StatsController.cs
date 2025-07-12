using System;
using System.Collections.Generic;
using Core;
using Scripts.GlobalStateMachine;
using Scripts.Meta;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Ui
{
    public class StatsController : IExecute, ICleanUp
    {
        private Action _onActiveStat;
        private ProgressDataAdapter _adapter;
        private readonly LocalEvents _localEvents;
        private List<IStatBarView> _views = new();
        private float _timeSinceLastUpdate;
        private float _updateInterval = 0.5f;

        public StatsController(ProgressDataAdapter adapter, LocalEvents localEvents)
        {
            _adapter = adapter;
            _localEvents = localEvents;
        }
    
        public void RegisterView(IStatBarView barView) {
            _views.Add(barView);
            barView.Init(_localEvents);
            var value = _adapter.GetStats(barView.MetaType);
            var maxValue = _adapter.GetMaxStats(barView.MetaType);
            barView.UpdateView(value, maxValue);
        }
        public void UpdateAllViews() 
        {
            foreach (var view in _views) {
                var value = _adapter.GetStats(view.MetaType);
                var maxValue = _adapter.GetMaxStats(view.MetaType);

                view.UpdateView(value, maxValue);
                Debug.Log(view.MetaType.ToString() + " - " +value);
            }
        }

        public void CleanUp()
        {
        }

        public void Execute(float deltaTime) 
        {
            _timeSinceLastUpdate += deltaTime;
            if (_timeSinceLastUpdate >= _updateInterval) 
            {
                UpdateAllViews();
                _timeSinceLastUpdate = 0;
            }
        }

        public void ShowTooltip(MetaType metaType)
        {
            
        }

        public void HideTooltip()
        {
            
        }
    }
}