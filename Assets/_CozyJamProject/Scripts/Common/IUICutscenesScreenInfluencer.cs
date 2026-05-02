using CozySpringJam.Game.SO;
using R3;

namespace CozySpringJam
{
    public interface IUICutscenesScreenInfluencer : IUIScreenInfluencer<CutscenesScreenSettings, (CutscenesScreenSettings, System.Action)>
    {
        public Subject<(float, System.Action)> PrepareFadeInSignal { get; }
        public Subject<float> FadeInSignal { get; }
        public Subject<(float, System.Action)> FadeOutSignal { get; }
    }
}
