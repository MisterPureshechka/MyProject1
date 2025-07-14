using Core;
using Scripts.Data;
using Scripts.GlobalStateMachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Scripts.EcoSystem
{
    public class VolumeLogic : ICleanUp
    {
        private readonly LocalEvents _localEvents;
        private readonly InteractiveObjectConfig _config;
        private Volume _volume;
        private ColorAdjustments _colorAdjustment;

        private Color _dayColor;
        private Color _nightColor;

        public VolumeLogic(LocalEvents localEvents, InteractiveObjectConfig config)
        {
            _localEvents = localEvents;
            _config = config;
            _volume = Object.FindAnyObjectByType<Volume>();

            InitColors();
            InitColorOverride();

            _localEvents.OnDayTimeChange += UpdateColorByTime;
        }

        private void InitColorOverride()
        {
            _colorAdjustment = ScriptableObject.CreateInstance<ColorAdjustments>();
            var components = _volume.sharedProfile.components;
            components.Add(_colorAdjustment);
        }

        private void InitColors()
        {
            _dayColor = _config.VolumeDayColor;
            _nightColor = _config.VolumeNightColor;
        }

        private void UpdateColorByTime(float value)
        {
            _colorAdjustment.colorFilter.overrideState = true;
            _colorAdjustment.colorFilter.value = Color.Lerp(_nightColor, _dayColor, value);
        }

        public void CleanUp()
        {
            _localEvents.OnDayTimeChange -= UpdateColorByTime;
        }
    }
}