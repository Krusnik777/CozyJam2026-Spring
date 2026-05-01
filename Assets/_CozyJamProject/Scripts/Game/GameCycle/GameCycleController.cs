using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleController : System.IDisposable
    {
        private GameCycleControllerView _view;

        public GameCycleController(GameCycleControllerView view)
        {
            _view = view;
        }

        public void Dispose()
        {
            
        }
    }
}
