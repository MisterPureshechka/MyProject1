using Scripts.GlobalStateMachine;
using Scripts.Meta;

namespace Scripts.Ui
{
    public interface IStatBarView
    {
        string DataKey { get; }
        void UpdateView(float value, float maxValue);
        void Init(LocalEvents localEvents);
        
        MetaType MetaType { get; }
        
    }
}