using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public sealed class SceneTransitionCurtain : MonoBehaviour
    {
        private static SceneTransitionCurtain instance;
        private CanvasGroup group;
        private Coroutine reveal;
        public static bool IsCovered => instance != null && instance.group.blocksRaycasts;

        public static void Cover()
        {
            if (instance == null)
            {
                var root = new GameObject("Scene transition", typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
                DontDestroyOnLoad(root);
                var canvas = root.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = short.MaxValue;
                instance = root.AddComponent<SceneTransitionCurtain>();
                instance.group = root.GetComponent<CanvasGroup>();
                var shade = new GameObject("Black", typeof(RectTransform), typeof(Image));
                shade.transform.SetParent(root.transform, false);
                var rect = (RectTransform)shade.transform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                shade.GetComponent<Image>().color = Color.black;
            }
            if (instance.reveal != null) instance.StopCoroutine(instance.reveal);
            instance.reveal = null;
            instance.group.alpha = 1;
            instance.group.blocksRaycasts = true;
        }

        public static void RevealWhenReady()
        {
            if (instance == null) return;
            if (instance.reveal != null) instance.StopCoroutine(instance.reveal);
            instance.reveal = instance.StartCoroutine(instance.Reveal());
        }

        private IEnumerator Reveal()
        {
            // Let scene Start callbacks bind HUD state, then settle the camera before rendering it.
            yield return null;
            SceneContext.CameraManager?.PrepareInitialCamera();
            Canvas.ForceUpdateCanvases();
            yield return null;
            for (float elapsed = 0; elapsed < 0.2f; elapsed += Time.unscaledDeltaTime)
            {
                group.alpha = 1 - elapsed / 0.2f;
                yield return null;
            }
            group.alpha = 0;
            group.blocksRaycasts = false;
            reveal = null;
        }
    }
}
