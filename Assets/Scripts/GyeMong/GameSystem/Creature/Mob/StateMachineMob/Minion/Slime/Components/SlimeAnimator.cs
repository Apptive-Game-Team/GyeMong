using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components
{
    public class SlimeAnimator : MonoBehaviour
    {
        public static float AnimationDeltaTime = 0.4f;
    
        private SpriteRenderer _spriteRenderer;
        private SlimeSprites _sprites;
        private Coroutine _currentAnimation;
        public AnimationType CurrentAnimationType { get; private set; }
        private bool _stopCurrentAnimation = false;
    
        public static SlimeAnimator Create(GameObject parent, SlimeSprites sprites)
        {
            SlimeAnimator animator = parent.GetComponent<SlimeAnimator>() == null ?
             parent.AddComponent<SlimeAnimator>() : parent.GetComponent<SlimeAnimator>();
            animator._spriteRenderer = parent.GetComponent<SpriteRenderer>();
            animator._sprites = sprites;
            return animator;
        }
        
        public void SetSprites(SlimeSprites sprites)
        {
            this._sprites = sprites;
        }

        public enum AnimationType
        {
            Idle,
            MeleeAttack,
            RangedAttack,
            DashAttack,
            Die
        }
    
        public IEnumerator SyncPlay(AnimationType type, bool loop = false, int dir = 0)
        {
            CurrentAnimationType = type;
            do
            {
                yield return PlayerAnimation(_sprites.GetSprite(type, dir));
            } while (loop);
        }
    

    
        public void AsyncPlay(AnimationType type, bool loop = false, int dir = 0)
        {
            Stop();
            _currentAnimation = StartCoroutine(SyncPlay(type, loop, dir));
        }
    
        public void Stop()
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            _currentAnimation = null;
        }

        private IEnumerator PlayerAnimation(Sprite[] sprites)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                _spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(AnimationDeltaTime);
            }
        }
    }
}
