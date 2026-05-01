using DI;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.EntryPoints
{
    public class MainMenuEntryPoint : EntryPoint
    {
        private Subject<string> _onEnd;

        public override Observable<string> Run(DIContainer sceneContainer)
        {
            Debug.Log("ENTRY POINT: Main Menu");

            _onEnd = new();

            return _onEnd;
        }
    }
}
