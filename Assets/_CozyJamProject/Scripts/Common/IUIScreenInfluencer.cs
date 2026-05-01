using R3;

namespace CozySpringJam
{
    public interface IUIScreenInfluencer<T> where T : class
    {
        public Subject<T> ShowSignal { get; }
        public Subject<Unit> HideSignal { get; }
    }
}
