using R3;

namespace CozySpringJam
{
    public interface IUIScreenInfluencer<T, K>
    {
        public Subject<T> ShowSignal { get; }
        public Subject<K> HideSignal { get; }
    }
}
