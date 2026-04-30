using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CozySpringJam.Utils;

namespace CozySpringJam.Game.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private Coroutines _coroutines;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutostartGame()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
        }

        private /*async*/ void RunGame()
        {
            /*#if UNITY_EDITOR

            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == Scenes.GAMEPLAY)
            {
                var enterParams = new GameplayEnterParams(0);
                _coroutines.StartCoroutine(LoadAndStartGameplay(enterParams));

                return;
            }

            if (sceneName == Scenes.MAIN_MENU)
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());

                return;
            }

            if (sceneName != Scenes.BOOTSTRAP)
            {
                return;
            }

            #endif*/

            _coroutines.StartCoroutine(LoadAndStartMainMenu());
        }

        private IEnumerator LoadAndStartGameplay(/*GameplayEnterParams enterParams*/)
        {
            //_uiRoot.ShowLoadingScreen();
            //_cachedSceneContainer?.Dispose(); // Or any other cleanup

            yield return LoadScene(Scenes.BOOTSTRAP);
            yield return LoadScene(Scenes.GAMEPLAY);

            yield return new WaitForSeconds(1);

            // Loading Saves for scene if has

            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint>();
            sceneEntryPoint.Run(/*Some Params*//*And Exit Func*/);

            //_uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartMainMenu(/*MainMenuEnterParams enterParams = null*/)
        {
            //_uiRoot.ShowLoadingScreen();
            //_cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOTSTRAP);
            yield return LoadScene(Scenes.MAIN_MENU);

            yield return new WaitForSeconds(1);

            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint>();
            sceneEntryPoint.Run(/*Some Params*//*And Exit Func*/);

            //_uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
