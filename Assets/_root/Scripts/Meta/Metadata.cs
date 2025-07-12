using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scripts.Meta
{
    [Serializable]
    public class Metadata
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MetaType MetaType;

        public float Value;

        public float MaxValue;

        public string DisplayName;

        public string Tooltip;

        public virtual void ChangeValue(float delta)
        {
            Value += delta;
            Value = Math.Clamp(Value, 0, MaxValue); 
        }

        public virtual void Init(float value)
        {
            Value = Math.Clamp(value, 0, MaxValue);
        }
    }
}