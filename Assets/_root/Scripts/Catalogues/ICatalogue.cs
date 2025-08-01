using System;

namespace Scripts.Catalogues
{
    public interface ICatalogue
    {
        void Show(Action onComplete = null);
        void Hide(Action onComplete = null);
        bool IsVisible { get; }
    }
}