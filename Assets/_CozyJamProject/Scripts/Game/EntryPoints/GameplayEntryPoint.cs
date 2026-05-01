using CozySpringJam.Game.GameCycle;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint
    {
        [SerializeField] private GameCycleControllerView m_gameCycleControllerView;

        private GameCycleController _gameCycleController;

        private Subject<string> _onEnd;

        private System.IDisposable _gameCycleFinishListener;

        public override Observable<string> Run()
        {
            Debug.Log("ENTRY POINT: Started Gameplay");

            _gameCycleController = new GameCycleController(m_gameCycleControllerView);

            _onEnd = new();

            _gameCycleFinishListener = _gameCycleController.OnFinish.Subscribe(_ => FinishGame());

            return _onEnd;
        }

        private void OnDestroy()
        {
            _gameCycleController?.Dispose();
        }

        private void FinishGame()
        {
            _gameCycleController?.Dispose();

            _onEnd.OnNext("FINISH");
        }
    }
}
