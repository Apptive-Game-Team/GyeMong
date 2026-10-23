using System;
using System.Collections.Generic;
using UnityEngine;
using GyeMong.InputSystem;

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
            string result = Template(source);
            if (result.IndexOf('{') < 0) return result;
            foreach (var binding in InputManager.Instance.GetKeyActions())
                result = result.Replace("{" + binding.Key + "}", "<b>[" + KeyLabel(binding.Value) + "]</b>");
            return result;
        }

        public static string Template(string source)
        {
            Load();
            if (source == null) return string.Empty;
            if (!entries.TryGetValue(source, out var entry)) return source;
            string result = Language == GameLanguage.English ? entry.en : entry.ko;
            result = string.IsNullOrEmpty(result) ? source : result;
            return result;
        }

        public static string KeyLabel(KeyCode key)
        {
            bool english = Language == GameLanguage.English;
            switch (key)
            {
                case KeyCode.None: return english ? "Unbound" : "미지정";
                case KeyCode.Mouse0: return english ? "Left Click" : "마우스 좌클릭";
                case KeyCode.Mouse1: return english ? "Right Click" : "마우스 우클릭";
                case KeyCode.Mouse2: return english ? "Middle Click" : "마우스 휠 클릭";
                case KeyCode.LeftShift: return "Left Shift";
                case KeyCode.RightShift: return "Right Shift";
                default: return key.ToString();
            }
        }
    }
}
