using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public class Indicator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        public IEnumerator Flicker(float duration)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Color originalColor = _spriteRenderer.color;
            Color highAlpha = new Color(originalColor.r, originalColor.g, originalColor.b, 0.8f);
            Color lowAlpha = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);

            float elapsed = 0f;
            float flickerStart = duration * (2f / 3f);

            float flickerInterval = 0.1f;
            float flickerTimer = 0f;
            bool isHigh = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                if (elapsed >= flickerStart)
                {
                    flickerTimer += Time.deltaTime;
                    if (flickerTimer >= flickerInterval)
                    {
                        flickerTimer = 0f;
                        isHigh = !isHigh;
                        _spriteRenderer.color = isHigh ? highAlpha : lowAlpha;
                    }
                }

                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}
