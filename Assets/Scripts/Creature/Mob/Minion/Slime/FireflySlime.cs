using System.Collections;
using Creature.Mob.Minion.Slime;
using Map.Puzzle.FogMaze;
using UnityEngine;

namespace Creature.Minion.Slime
{
    public class FireflySlime : SlimeBase
    {
        [SerializeField] private SlimeSprites _spriteOnVersion;
        [SerializeField] private SlimeSprites _spriteOffVersion;
        [SerializeField] private GameObject _light;
        
        private bool _isOn = true;
        private const float LIGHT_DELAY = 5f;
        private FogController _fogController;
        
        private Coroutine _lightCoroutine;

        protected override void Initialize()
        {
            _fogController = FindObjectOfType<FogController>();
            _fogController.AddTransform(transform);
            
            _light.SetActive(_isOn);
            base.Initialize();
        }

        public override void StartMob()
        {
            _isOn = true;
            _light.SetActive(_isOn);
            _slimeAnimator.SetSprites(_spriteOnVersion);
        }

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
            _light.SetActive(false);
            _fogController.RemoveTransform(transform);
            _slimeAnimator.SetSprites(_spriteOffVersion);
            yield return new WaitForSeconds(LIGHT_DELAY);
            _isOn = true;
            _light.SetActive(true);
            _fogController.AddTransform(transform);
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
        public class SlimeIdleState : IdleState { }
    }
}
