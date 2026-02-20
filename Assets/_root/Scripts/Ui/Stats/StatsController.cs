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
        private ProgressDataAdapterOLD _adapterOld;
        private readonly LocalEvents _localEvents;
        private List<IStatBarView> _views = new();
        private float _timeSinceLastUpdate;
        private float _updateInterval = 0.5f;

        public StatsController(ProgressDataAdapterOLD adapterOld, LocalEvents localEvents)
        {
            _adapterOld = adapterOld;
            _localEvents = localEvents;
        }
    
        public void RegisterView(IStatBarView barView) {
            _views.Add(barView);
            barView.Init(_localEvents);
            var value = _adapterOld.GetStats(barView.MetaType);
            var maxValue = _adapterOld.GetMaxStats(barView.MetaType);
            barView.UpdateView(value, maxValue);
        }
        public void UpdateAllViews() 
        {
            foreach (var view in _views) {
                var value = _adapterOld.GetStats(view.MetaType);
                var maxValue = _adapterOld.GetMaxStats(view.MetaType);

                view.UpdateView(value, maxValue);
            }
        }

        public void CleanUp()
        {
            _views.Clear();
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