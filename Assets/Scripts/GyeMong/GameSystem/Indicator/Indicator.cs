using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public class Indicator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        public IEnumerator Flick(float duration)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Color originalColor = _spriteRenderer.color;
            Color lowAlpha = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
            
            int flickCount = 6;
            float flickInterval = 0.1f;
            float flickStart = duration - flickInterval * flickCount;
            float elapsed = 0f;

            while (elapsed < flickStart)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < flickCount; i++)
            {
                _spriteRenderer.color = (i % 2 == 0) ? lowAlpha : originalColor;
                yield return new WaitForSeconds(flickInterval);
            }

            Destroy(gameObject);
        }
    }
}
