using GyeMong.InputSystem;
using GyeMong.UISystem.Option;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util
{
    // Scene files retain content; shared services and scene UI have explicit ownership.
    [DefaultExecutionOrder(-10000)]
    public sealed class SceneRuntimeInstaller : MonoBehaviour
    {
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject optionPrefab;
        [SerializeField] private GameObject hudPrefab;
        [SerializeField] private GameObject chatPrefab;

        private void Awake()
        {
            SceneTransitionCurtain.Cover();
            if (FindObjectOfType<InputManager>(true) == null && gameManagerPrefab != null)
                Create(gameManagerPrefab);
            if (FindObjectOfType<OptionUIToggler>(true) == null && optionPrefab != null)
                Create(optionPrefab);
            if (FindObjectOfType<EventSystem>(true) == null)
            {
                var events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
            if (hudPrefab != null)
            {
                var hud = Create(hudPrefab);
                // Dialogue is owned by ChatCanvas. Retired HUD dialogue must never render.
                var legacyChat = hud.transform.Find("ChatWindow");
                if (legacyChat != null) legacyChat.gameObject.SetActive(false);
            }
            if (chatPrefab != null) Create(chatPrefab);
            if (SceneLoader.Instance == null || !SceneLoader.Instance.IsLoading)
                SceneTransitionCurtain.RevealWhenReady();
        }

        private static GameObject Create(GameObject prefab)
        {
            var instance = Instantiate(prefab);
            instance.name = prefab.name;
            return instance;
        }

        private void Start() { SceneContext.CameraManager?.PrepareInitialCamera(); }
    }
}
