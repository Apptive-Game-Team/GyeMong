using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class Indicator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private float _flickDuration;
        private float _flickDelay;
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _flickDuration = 0.15f;
            _flickDelay = 0.25f;

            StartCoroutine(Flicker(_flickDuration, _flickDelay));
        }

        private IEnumerator Flicker(float duration, float delay)
        {
            Color originalColor = _spriteRenderer.color;
            Color highAlpha = new Color(originalColor.r, originalColor.g, originalColor.b, 0.8f);
            
            while (true)
            {
                _spriteRenderer.color = highAlpha;
                yield return new WaitForSeconds(duration);
                _spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
