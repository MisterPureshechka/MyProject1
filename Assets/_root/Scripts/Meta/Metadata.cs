using System;

namespace Scripts.Meta
{
    [Serializable]
    public class Metadata
    {
        public MetaType MetaType;
        public float Value;
        public string DisplayName; 
        public float MaxValue;    
        public string Tooltip;
        
        public virtual void ChangeValue(float value)
        {
            Value += value;
        }

        public virtual void Init(float value)
        {
            Value = value;
        }
        
    }
}