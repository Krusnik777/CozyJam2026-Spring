using CozySpringJam.Game.GameCycle;
using CozySpringJam.Game.Root;
using CozySpringJam.Game.Services;
using CozySpringJam.UI;
using DI;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint
    {
        [SerializeField] private UIGameplayRootView m_sceneUIRootPrefab;
        [SerializeField] private GameCycleControllerView m_gameCycleControllerView;

        private GameCycleController _gameCycleController;

        private Subject<string> _onEnd;

        private System.IDisposable _gameCycleFinishListener;

        public override Observable<string> Run(DIContainer sceneContainer)
        {
            Debug.Log("ENTRY POINT: Started Gameplay");

            var uiRoot = sceneContainer.Resolve<UIRootView>();
            var uiSceneRoot = Instantiate(m_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiSceneRoot.gameObject);

            _gameCycleController = new GameCycleController(m_gameCycleControllerView, sceneContainer.Resolve<MessageService>());
            uiSceneRoot.Construct(_gameCycleController);

            _onEnd = new();

            _gameCycleFinishListener = _gameCycleController.OnFinish.Subscribe(_ => FinishGame());

            return _onEnd;
        }

        private void OnDestroy()
        {
            DisposeOfListeners();
        }

        private void FinishGame()
        {
            DisposeOfListeners();

            _onEnd.OnNext("FINISH");
        }

        private void DisposeOfListeners()
        {
            _gameCycleController?.Dispose();
            _gameCycleFinishListener?.Dispose();
        }
    }
}
