using System;
using CozySpringJam.Game.GameCycle;
using R3;

namespace CozySpringJam
{
    public interface IUIPicturesScreenInfluencer : IUIScreenInfluencer<PicturesScreenSettings, Unit>
    {
        public Subject<(Action, Action)> UpperZoomSignal { get; }
        public Subject<(Action, Action)> LowerZoomSignal { get; }
    }
}
