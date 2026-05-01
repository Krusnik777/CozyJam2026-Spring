using CozySpringJam.Game.GameCycle;
using UnityEngine;

namespace CozySpringJam.UI
{
    public class UIGameplayRootView : MonoBehaviour
    {
        [SerializeField] private Transform m_screensTransform;

        private UIPicturesScreen _uiPicturesScreen;

        public void Construct(IUIScreenInfluencer<PicturesScreenSettings> picturesScreenInfluencer)
        {
            var uiPicturesScreenPrefab = Resources.Load<UIPicturesScreen>("Prefabs/UI/PicturesScreen");
            _uiPicturesScreen = Instantiate(uiPicturesScreenPrefab, m_screensTransform);
            _uiPicturesScreen.Setup(picturesScreenInfluencer);
        }

        private void OnDestroy()
        {
            if (_uiPicturesScreen) _uiPicturesScreen.Dispose(); // if Picture Screen is Disabled
        }
    }
}
