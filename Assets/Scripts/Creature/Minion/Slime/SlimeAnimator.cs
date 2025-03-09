using System.Collections;
using UnityEngine;

namespace Creature.Minion.Slime
{
    public class SlimeAnimator : MonoBehaviour
    {
        public static float ANIMATION_DELTA_TIME = 0.4f;
    
        private SpriteRenderer spriteRenderer;
        private SlimeSprites sprites;
        private Coroutine currentAnimation;
        public AnimationType CurrentAnimationType { get; private set; }
        private bool stopCurrentAnimation = false;
    
        public static SlimeAnimator Create(GameObject parent, SlimeSprites sprites)
        {
            SlimeAnimator animator = parent.gameObject.AddComponent<SlimeAnimator>();
            animator.spriteRenderer = parent.GetComponent<SpriteRenderer>();
            animator.sprites = sprites;
            return animator;
        }

        public enum AnimationType
        {
            IDLE,
            MELEE_ATTACK,
            RANGED_ATTACK,
            DIE
        }
    
        public IEnumerator SyncPlay(AnimationType type, bool loop = false)
        {
            CurrentAnimationType = type;
            do
            {
                yield return PlayerAnimation(sprites.GetSprite(type));
            } while (loop);
        }
    

    
        public void AsyncPlay(AnimationType type, bool loop = false)
        {
            if (currentAnimation != null)
            {
                Stop();
            }
            currentAnimation = StartCoroutine(SyncPlay(type, loop));
        }
    
        public void Stop()
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        private IEnumerator PlayerAnimation(Sprite[] sprites)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(ANIMATION_DELTA_TIME);
            }
        }
    }
}
