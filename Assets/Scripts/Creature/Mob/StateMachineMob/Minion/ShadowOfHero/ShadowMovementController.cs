using System.Collections;
using System.Event.Interface;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class ShadowMovementController : MonoBehaviour, IControllable
    {
        private Animator _animator;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        public IEnumerator MoveTo(Vector3 target, float speed)
        {
            _animator.SetBool("isMove", true);
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                _animator.SetFloat("xDir", target.x - transform.position.x);
                _animator.SetFloat("yDir", target.y - transform.position.y);
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }
            _animator.SetBool("isMove", false);
        }
    }
}