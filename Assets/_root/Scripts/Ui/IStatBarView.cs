using Scripts.Meta;

namespace Scripts.Ui
{
    public interface IStatBarView
    {
        string DataKey { get; }
        void UpdateView(float value, float maxValue);
        MetaType Metatype { get; }
    }
}