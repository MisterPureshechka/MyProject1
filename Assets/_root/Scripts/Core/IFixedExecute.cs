
namespace Core
{
    internal interface IFixedExecute : IController
    {
        void FixedExecute(float fixedDeltaTime);
    }
}
