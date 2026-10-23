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
        private int _direction;
        private int _frameIndex;
        private bool _isPlaying;
    
        public static SlimeAnimator Create(GameObject parent, SlimeSprites sprites)
        {
            SlimeAnimator animator = parent.GetComponent<SlimeAnimator>() == null ?
             parent.AddComponent<SlimeAnimator>() : parent.GetComponent<SlimeAnimator>();
            animator._spriteRenderer = parent.GetComponentInChildren<SpriteRenderer>();
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
            _direction = dir;
            _frameIndex = 0;
            _isPlaying = true;
            do
            {
                yield return PlayerAnimation(type);
            } while (loop);
            _isPlaying = false;
        }

        public void SetDirection(int direction)
        {
            if (_direction == direction) return;
            _direction = direction;
            if (!_isPlaying) return;
            var frames = _sprites.GetSprite(CurrentAnimationType, _direction);
            _spriteRenderer.sprite = frames[Mathf.Min(_frameIndex, frames.Length - 1)];
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
            _isPlaying = false;
        }

        private IEnumerator PlayerAnimation(AnimationType type)
        {
            for (int i = 0; i < _sprites.GetSprite(type, _direction).Length; i++)
            {
                _frameIndex = i;
                _spriteRenderer.sprite = _sprites.GetSprite(type, _direction)[i];
                yield return new WaitForSeconds(AnimationDeltaTime);
            }
        }
    }
}
