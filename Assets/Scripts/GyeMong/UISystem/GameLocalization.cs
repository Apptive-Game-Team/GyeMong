using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.UISystem
{
    public enum GameLanguage { Korean, English }

    public static class GameLocalization
    {
        public const string PreferenceKey = "GyeMong.Language";
        private static Dictionary<string, Entry> entries;
        private static GameLanguage? language;
        public static event Action Changed;
        public static GameLanguage Language => language ?? (language =
            PlayerPrefs.GetInt(PreferenceKey, 0) == 1 ? GameLanguage.English : GameLanguage.Korean).Value;

        [Serializable] public class Entry { public string source; public string ko; public string en; }
        [Serializable] private class Catalogue { public Entry[] entries; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            language = null;
            entries = null;
            Changed = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
                    if (HasTranslation(text.text)) LocalizedGameText.Bind(text, text.text);
                foreach (var image in root.GetComponentsInChildren<UnityEngine.UI.Image>(true))
                {
                    if (image.GetComponent<TItle.StartButton>() != null) LocalizedImageText.Bind(image, "새 게임");
                    else if (image.GetComponent<TItle.ContinueButton>() != null) LocalizedImageText.Bind(image, "이어하기");
                    else if (image.GetComponent<TItle.OptionButton>() != null) LocalizedImageText.Bind(image, "설정");
                    else if (image.GetComponent<TItle.ExitButton>() != null) LocalizedImageText.Bind(image, "게임 종료");
                }
            }
        }

        public static void SetLanguage(GameLanguage value, bool persist = true)
        {
            language = value;
            if (persist)
            {
                PlayerPrefs.SetInt(PreferenceKey, (int)value);
                PlayerPrefs.Save();
            }
            Changed?.Invoke();
        }

        private static void Load()
        {
            if (entries != null) return;
            entries = new Dictionary<string, Entry>(StringComparer.Ordinal);
            var asset = Resources.Load<TextAsset>("Localization/GameText");
            if (asset == null) return;
            foreach (var entry in JsonUtility.FromJson<Catalogue>(asset.text).entries)
                entries[entry.source] = entry;
        }

        public static bool HasTranslation(string source)
        {
            Load();
            return source != null && entries.ContainsKey(source);
        }

        public static string Text(string source)
        {
            Load();
            if (source == null) return string.Empty;
            if (!entries.TryGetValue(source, out var entry)) return source;
            string result = Language == GameLanguage.English ? entry.en : entry.ko;
            return string.IsNullOrEmpty(result) ? source : result;
        }
    }
}
