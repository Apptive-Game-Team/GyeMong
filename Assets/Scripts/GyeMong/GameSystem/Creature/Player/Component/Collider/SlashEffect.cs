using UnityEngine;

namespace GyeMong.GameSystem.Creature.Player.Component.Collider
{
    public class SlashEffect : MonoBehaviour
    {
        private const float SlashEffectDuration = 0.25f;
        private void Start()
        {
            int order = GetComponentInParent<SpriteRenderer>().sortingOrder + 1;
            GetComponent<SpriteRenderer>().sortingOrder = order;
            GetComponentInChildren<ParticleSystem>().GetComponent<Renderer>().sortingOrder = order;
            Destroy(gameObject, SlashEffectDuration);
        }
    }
}
