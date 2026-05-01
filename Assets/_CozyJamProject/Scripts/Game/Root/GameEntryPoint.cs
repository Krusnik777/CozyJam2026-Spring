using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CozySpringJam.Utils;
using CozySpringJam.Game.EntryPoints;
using R3;
using DI;
using CozySpringJam.Game.Services;
using CozySpringJam.Game.SO;
using Unity.VisualScripting;

namespace CozySpringJam.Game.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private Coroutines _coroutines;
        private UIRootView _uiRoot;

        private readonly DIContainer _rootContainer = new();
        private DIContainer _cachedSceneContainer;

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

            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot);

            var messageService = new MessageService(_uiRoot.UIMessagePanelView);
            _rootContainer.RegisterInstance(messageService);
            
            ParticleCollections collections = Resources.Load<ParticleCollections>("ParticleCollection");
            ParticleService particleService = new ParticleService(collections);
            _rootContainer.RegisterInstance(particleService);

            AudioSource audioSource = _coroutines.AddComponent<AudioSource>();
            AudioSource backgroundMusic = new GameObject("[BACKGROUND_MUSIC]").AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(backgroundMusic);
            
            SoundService soundService = new SoundService(audioSource, backgroundMusic);
            _rootContainer.RegisterInstance(soundService);

            var cutscenesScreenSettings = Resources.Load<CutscenesScreenSettings>("CutscenesScreenSettings");
            var cutsceneService = new CutsceneService(cutscenesScreenSettings);
            _rootContainer.RegisterInstance(cutsceneService);
        }

        private /*async*/ void RunGame()
        {
            #if UNITY_EDITOR

            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == Scenes.GAMEPLAY)
            {
                //var enterParams = new GameplayEnterParams(0);
                _coroutines.StartCoroutine(LoadAndStartGameplay(/*enterParams*/));

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

            #endif

            //_coroutines.StartCoroutine(LoadAndStartMainMenu());
            _coroutines.StartCoroutine(LoadAndStartGameplay());
        }

        private IEnumerator LoadAndStartGameplay(/*GameplayEnterParams enterParams*/)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOTSTRAP);
            yield return LoadScene(Scenes.GAMEPLAY);

            yield return new WaitForSeconds(1);

            // Loading Saves for scene if has any

            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer).Subscribe(exitTag =>
            {
                if (exitTag == "FINISH")
                {
                    //Application.Quit();
                    _coroutines.StartCoroutine(LoadAndStartGameplay());
                    return;
                }

                _coroutines.StartCoroutine(LoadAndStartMainMenu());
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartMainMenu(/*MainMenuEnterParams enterParams = null*/)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOTSTRAP);
            yield return LoadScene(Scenes.MAIN_MENU);

            yield return new WaitForSeconds(1);

            // Loading Saves for scene if has any and is needed

            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer).Subscribe(exitTag =>
            {
                if (exitTag == Scenes.GAMEPLAY) _coroutines.StartCoroutine(LoadAndStartGameplay());
                //else Application.Quit();
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
