using CozySpringJam.Game.GameCycle;
using CozySpringJam.Game.SO;
using R3;
using UnityEngine;

namespace CozySpringJam.UI
{
    public class UIGameplayRootView : MonoBehaviour
    {
        [SerializeField] private Transform m_screensTransform;

        private UIPicturesScreen _uiPicturesScreen;
        private UICutscenesScreen _uICutscenesScreen;

        public void BindScreen(IUIScreenInfluencer<PicturesScreenSettings, Unit> picturesScreenInfluencer)
        {
            var uiPicturesScreenPrefab = Resources.Load<UIPicturesScreen>("Prefabs/UI/PicturesScreen");
            _uiPicturesScreen = Instantiate(uiPicturesScreenPrefab, m_screensTransform);
            _uiPicturesScreen.Setup(picturesScreenInfluencer);
        }

        public void BindScreen(IUICutscenesScreenInfluencer cutsceneScreenInfluencer)
        {
            var uiCutscenesScreenPrefab = Resources.Load<UICutscenesScreen>("Prefabs/UI/CutscenesScreen");
            _uICutscenesScreen = Instantiate(uiCutscenesScreenPrefab, m_screensTransform);
            _uICutscenesScreen.Setup(cutsceneScreenInfluencer);
        }

        private void OnDestroy()
        {
            if (_uiPicturesScreen) _uiPicturesScreen.Dispose(); // if Picture Screen is Disabled
            if (_uICutscenesScreen) _uICutscenesScreen.Dispose(); // if Cutscene Screen is Disabled
        }
    }
}
