using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GyeMong.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem
{
    // Figma's pixel-art controls, with labels taken from the current input bindings.
    public sealed class TutorialInputDisplay : MonoBehaviour
    {
        private TMP_FontAsset font;
        private string source;
        private RectTransform controls;
        private RectTransform advance;

        public void Show(TMP_FontAsset textFont, string content)
        {
            font = textFont;
            source = content;
            Refresh();
        }

        private void OnEnable() { GameLocalization.Changed += Refresh; }
        private void OnDisable() { GameLocalization.Changed -= Refresh; }

        private void Refresh()
        {
            if (font == null || source == null) return;
            Clear(controls);
            Clear(advance);
            controls = Rect("InputIllustration", transform, Vector2.zero, new Vector2(700, 220));
            controls.anchoredPosition = new Vector2(0, 110);
            var bindings = InputManager.Instance.GetKeyActions();
            var keys = new List<KeyCode>();
            foreach (Match token in Regex.Matches(GameLocalization.Template(source), @"\{(\w+)\}"))
                if (Enum.TryParse(token.Groups[1].Value, out ActionCode action) && bindings.TryGetValue(action, out var key))
                    keys.Add(key);

            if (keys.Count == 4 && keys.TrueForAll(key => key < KeyCode.Mouse0 || key > KeyCode.Mouse6))
            {
                var movement = Picture("Movement", controls, Vector2.zero, new Vector2(233, 176));
                var positions = new[] { new Vector2(0, 48), new Vector2(-80, -43), new Vector2(0, -43), new Vector2(80, -43) };
                for (int i = 0; i < 4; i++) Label(movement, GameLocalization.KeyLabel(keys[i]), positions[i], new Vector2(65, 52), 28, Color.black);
            }
            else
                for (int i = 0; i < keys.Count; i++)
                    Control(controls, keys[i], new Vector2((i - (keys.Count - 1) / 2f) * 190, 0));
            controls.gameObject.SetActive(keys.Count > 0);

            // Space remains a valid example; advancing also accepts other keys/clicks.
            var oldHint = transform.Find("Content (1)");
            if (oldHint != null) oldHint.gameObject.SetActive(false);
            advance = Rect("AdvanceIllustration", transform, Vector2.zero, new Vector2(620, 51));
            advance.anchorMin = advance.anchorMax = new Vector2(.5f, 0);
            advance.anchoredPosition = new Vector2(0, 102.5f);
            var space = Picture("Space", advance, new Vector2(-175, 0), new Vector2(147, 51));
            Label(space, "space", new Vector2(0, 6), new Vector2(130, 36), 24, Color.black);
            Label(advance, GameLocalization.Language == GameLanguage.English ? "or any key to continue" : "또는 아무 키로 넘기기", new Vector2(95, 0), new Vector2(360, 46), 24, Color.white);
        }

        private void Control(Transform parent, KeyCode key, Vector2 position)
        {
            if (key == KeyCode.Mouse0 || key == KeyCode.Mouse1)
            {
                var mouse = Picture("Mouse", parent, position, new Vector2(93, 145)).GetComponent<RawImage>();
                // The downloaded texture highlights the left button; mirror it for right click.
                mouse.uvRect = key == KeyCode.Mouse0 ? new Rect(0, 0, 1, 1) : new Rect(1, 0, -1, 1);
                return;
            }
            var cap = Picture("Key", parent, position, new Vector2(157, 87));
            string label = key == KeyCode.LeftShift ? "L Shift" : key == KeyCode.RightShift ? "R Shift" : GameLocalization.KeyLabel(key);
            Label(cap, label, new Vector2(0, 8), new Vector2(139, 55), 30, Color.black);
        }

        private static RectTransform Picture(string asset, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = Rect(asset, parent, position, size);
            var image = rect.gameObject.AddComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>("TutorialInput/" + asset);
            if (image.texture != null) image.texture.filterMode = FilterMode.Point;
            image.raycastTarget = false;
            return rect;
        }

        private void Label(Transform parent, string value, Vector2 position, Vector2 size, float fontSize, Color color)
        {
            var text = Rect("Label", parent, position, size).gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.text = value;
            text.fontSize = text.fontSizeMax = fontSize;
            text.fontSizeMin = 14;
            text.enableAutoSizing = true;
            text.alignment = TextAlignmentOptions.Center;
            text.color = color;
            text.raycastTarget = false;
        }

        private static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private static void Clear(RectTransform root)
        {
            if (root == null) return;
            root.gameObject.SetActive(false);
            if (Application.isPlaying) Destroy(root.gameObject);
            else DestroyImmediate(root.gameObject);
        }
    }
}
