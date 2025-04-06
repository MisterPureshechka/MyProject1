using System;
using System.Collections.Generic;
using Core;
using Scripts.Progress;
using UnityEngine;

namespace Scripts.Ui
{
    public class StatsController : IExecute, ICleanUp
    {
        private Action _onActiveStat;
        private ProgressDataAdapter _adapter;
        private List<IStatBarView> _views = new();
        private float _timeSinceLastUpdate;
        private float _updateInterval = 0.5f;

        public StatsController(ProgressDataAdapter adapter) {
            _adapter = adapter;
        }
    
        public void RegisterView(IStatBarView barView) {
            _views.Add(barView);
            var value = _adapter.GetStats(barView.Metatype);
            var maxValue = _adapter.GetMaxStats(barView.Metatype);
            barView.UpdateView(value, maxValue);
        }
    
        public void UpdateAllViews() {
            foreach (var view in _views) {
                var value = _adapter.GetStats(view.Metatype);
                var maxValue = _adapter.GetMaxStats(view.Metatype);

                view.UpdateView(value, maxValue);
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
    }
}