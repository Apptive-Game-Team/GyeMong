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
            var label = LocalizedLabelFactory.CreateLabel(image.transform, LocalizedLabelFactory.Font, 44);
            label.name = "English Label";
            label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
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
