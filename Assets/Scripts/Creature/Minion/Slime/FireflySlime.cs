using System.Collections;
using UnityEngine;

namespace Creature.Minion.Slime
{
    public class FireflySlime : SlimeBase
    {
        [SerializeField] private SlimeSprites _spriteOnVersion;
        [SerializeField] private SlimeSprites _spriteOffVersion;
        private bool _isOn = true;
        private const float LIGHT_DELAY = 5f;
        
        private Coroutine _lightCoroutine;

        public override void OnAttacked(float damage)
        {
            SwapSprites();
            base.OnAttacked(damage);
        }

        private void SwapSprites()
        {
            if (_lightCoroutine != null)
            {
                StopCoroutine(_lightCoroutine);
            }
            _lightCoroutine = StartCoroutine(CountSwapSprites());
        }
        
        private IEnumerator CountSwapSprites()
        {
            _isOn = false;
            _slimeAnimator.SetSprites(_spriteOffVersion);
            yield return new WaitForSeconds(LIGHT_DELAY);
            _isOn = true;
            _slimeAnimator.SetSprites(_spriteOnVersion);
        }

        public class SlimeRangedAttackState : RangedAttackState
        {
            public override int GetWeight()
            {
                if (((FireflySlime)Slime)._isOn) // only attack when the slime is on
                {
                    return base.GetWeight();
                }
                return 0;
            }
        }
        
        public class SlimeMeleeAttackState : MeleeAttackState {}
        public class DieState : SlimeDieState { }
        public class MoveState : SlimeMoveState { }
    }
}
