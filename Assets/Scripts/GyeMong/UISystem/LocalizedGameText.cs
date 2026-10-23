using TMPro;
using UnityEngine;

namespace GyeMong.UISystem
{
    // Retains the authored source so switching back never translates a translation.
    public sealed class LocalizedGameText : MonoBehaviour
    {
        private TMP_Text target;
        private string source;

        public static void Bind(TMP_Text target, string source)
        {
            var binding = target.GetComponent<LocalizedGameText>();
            if (binding == null) binding = target.gameObject.AddComponent<LocalizedGameText>();
            binding.target = target;
            binding.source = source;
            binding.Refresh();
        }

        private void OnEnable() { GameLocalization.Changed += Refresh; Refresh(); }
        private void OnDisable() { GameLocalization.Changed -= Refresh; }
        private void Refresh()
        {
            if (target != null && source != null) target.text = GameLocalization.Text(source);
        }
    }
}
