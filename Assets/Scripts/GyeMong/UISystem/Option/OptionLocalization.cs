using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Option
{
    public static class OptionLocalization
    {
        public static void Install(Transform root)
        {
            var panel = root.Find("Canvas/OptionUI");
            if (panel == null || panel.Find("LanguageButton") != null) return;
            var font = LocalizedLabelFactory.Font;
            var imageLabels = new Dictionary<string, string> {
                {"OptionText", "설정"}, {"ResumeButton", "계속하기"},
                {"KeyMappingButton", "키 설정"}, {"SoundControlButton", "소리 설정"},
                {"OptionExitButton", "나가기"}, {"KeyMappingText", "키 설정"},
                {"SoundControlText", "소리 설정"}, {"KeyMappingExitButton", "뒤로"},
                {"SoundControlExitButton", "뒤로"}
            };
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (!imageLabels.TryGetValue(child.name, out var source)) continue;
                var image = child.GetComponent<Image>();
                if (image == null) continue;
                bool button = child.GetComponent<Button>() != null;
                image.sprite = null;
                image.color = button ? new Color(0.06f, 0.06f, 0.06f, 1) : Color.clear;
                var rect = (RectTransform)child;
                rect.sizeDelta = new Vector2(360, button ? 64 : 72);
                var label = LocalizedLabelFactory.CreateLabel(child, font, button ? 30 : 44);
                LocalizedGameText.Bind(label, source);
            }
            foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.GetComponent<LocalizedGameText>() != null)
                {
                    text.font = font;
                    text.fontSizeMax = text.fontSize;
                    text.fontSizeMin = 16;
                    text.enableAutoSizing = true;
                    text.enableWordWrapping = false;
                    continue;
                }
                if (!GameLocalization.HasTranslation(text.text)) continue;
                text.font = font;
                text.enableAutoSizing = true;
                text.fontSizeMin = 16;
                text.fontSizeMax = text.fontSize;
                text.enableWordWrapping = false;
                LocalizedGameText.Bind(text, text.text);
            }
            var language = new GameObject("LanguageButton", typeof(RectTransform), typeof(Image), typeof(Button));
            language.transform.SetParent(panel, false);
            var languageRect = (RectTransform)language.transform;
            languageRect.anchorMin = languageRect.anchorMax = new Vector2(0.5f, 0.5f);
            languageRect.anchoredPosition = new Vector2(0, -240);
            languageRect.sizeDelta = new Vector2(360, 56);
            language.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.06f, 1);
            var languageLabel = LocalizedLabelFactory.CreateLabel(language.transform, font, 26);
            LocalizedGameText.Bind(languageLabel, "언어: 한국어  /  English");
            language.GetComponent<Button>().onClick.AddListener(() =>
                GameLocalization.SetLanguage(GameLocalization.Language == GameLanguage.Korean ? GameLanguage.English : GameLanguage.Korean));
        }

    }
}
