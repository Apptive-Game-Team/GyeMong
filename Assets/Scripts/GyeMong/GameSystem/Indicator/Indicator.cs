using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public class Indicator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// 런타임에 만들어 이 인디케이터 전용으로 쓰는 머티리얼.
        /// 인디케이터가 사라질 때 같이 정리한다.
        /// </summary>
        private Material _ownedMaterial;

        public void SetOwnedMaterial(Material material)
        {
            _ownedMaterial = material;
        }

        public IEnumerator Flick(float duration)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Color originalColor = _spriteRenderer.color;
            Color lowAlpha = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
            
            int flickCount = 6;
            float flickInterval = 0.1f;
            if (flickCount * flickInterval > duration) flickCount = (int)(duration / flickInterval);
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

        private void OnDestroy()
        {
            if (_ownedMaterial != null)
            {
                Destroy(_ownedMaterial);
            }
        }
    }
}
