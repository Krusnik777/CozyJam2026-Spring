using CozySpringJam.Game.GameCycle;
using UnityEngine;

namespace CozySpringJam.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint
    {
        [SerializeField] private GameCycleControllerView m_gameCycleControllerView;

        private GameCycleController _gameCycleController;

        public override void Run()
        {
            Debug.Log("ENTRY POINT: Started Gameplay");

            _gameCycleController = new GameCycleController(m_gameCycleControllerView);
        }
    }
}
