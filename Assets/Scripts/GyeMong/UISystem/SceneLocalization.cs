using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.UISystem
{
    internal static class SceneLocalization
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
                    if (GameLocalization.HasTranslation(text.text)) LocalizedGameText.Bind(text, text.text);
                foreach (var image in root.GetComponentsInChildren<UnityEngine.UI.Image>(true))
                {
                    if (image.GetComponent<TItle.StartButton>() != null) LocalizedImageText.Bind(image, "새 게임");
                    else if (image.GetComponent<TItle.ContinueButton>() != null) LocalizedImageText.Bind(image, "이어하기");
                    else if (image.GetComponent<TItle.OptionButton>() != null) LocalizedImageText.Bind(image, "설정");
                    else if (image.GetComponent<TItle.ExitButton>() != null) LocalizedImageText.Bind(image, "게임 종료");
                }
            }
        }

    }
}
