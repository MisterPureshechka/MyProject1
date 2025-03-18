using System;

namespace Scripts.Metadata
{
    [Serializable]
    public abstract class Metadata
    {
        public float Value;

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