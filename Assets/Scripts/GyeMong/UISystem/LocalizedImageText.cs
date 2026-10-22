using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem
{
    // Keep the original Korean artwork; substitute live text for English.
    public sealed class LocalizedImageText : MonoBehaviour
    {
        private Image artwork;
        private Color originalColor;
        private TMP_Text label;
        private string source;

        public static void Bind(Image image, string source)
        {
            if (image.GetComponent<LocalizedImageText>() != null) return;
            var binding = image.gameObject.AddComponent<LocalizedImageText>();
            binding.artwork = image;
            binding.originalColor = image.color;
            binding.source = source;
            var label = new GameObject("English Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            label.transform.SetParent(image.transform, false);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
            label.font = Resources.Load<GameObject>("Prefabs/UI/ChatCanvas").transform.Find("NameArea").GetComponent<TMP_Text>().font;
            label.fontSize = label.fontSizeMax = 44;
            label.fontSizeMin = 16;
            label.enableAutoSizing = true;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            binding.label = label;
            binding.Refresh();
        }

        private void OnEnable() { GameLocalization.Changed += Refresh; Refresh(); }
        private void OnDisable() { GameLocalization.Changed -= Refresh; }
        private void Refresh()
        {
            if (artwork == null || label == null) return;
            bool english = GameLocalization.Language == GameLanguage.English;
            artwork.color = english ? Color.clear : originalColor;
            label.enabled = english;
            label.text = GameLocalization.Text(source);
        }
    }
}
