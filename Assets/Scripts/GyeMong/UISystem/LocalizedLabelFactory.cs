using TMPro;
using UnityEngine;

namespace GyeMong.UISystem
{
    internal static class LocalizedLabelFactory
    {
        public static TMP_FontAsset Font => Resources.Load<GameObject>("Prefabs/UI/ChatCanvas").transform.Find("NameArea").GetComponent<TMP_Text>().font;

        public static TMP_Text CreateLabel(Transform parent, TMP_FontAsset font, float size)
        {
            var label = new GameObject("Localized Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            label.transform.SetParent(parent, false);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(12, 0);
            label.rectTransform.offsetMax = new Vector2(-12, 0);
            label.font = font;
            label.fontSize = size;
            label.enableAutoSizing = true;
            label.fontSizeMin = 16;
            label.fontSizeMax = size;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            return label;
        }
    }
}
