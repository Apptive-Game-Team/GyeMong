using System.Collections;
using System.Event.Interface;
using UnityEngine;

namespace Creature.Event
{
    public class EventCreature : MonoBehaviour, IControllable
    {   
        Animator animator;
        Rigidbody2D rB;
        private void Start()
        {
            animator = GetComponent<Animator>();
            rB = GetComponent<Rigidbody2D>();
        }

        public IEnumerator MoveTo(Vector3 target, float speed)
        {
            rB.velocity = Vector2.zero;
            animator.SetBool("isMove", true);
            
            while (true)
            {
                Vector3 direction = (target - transform.position).normalized;
                
                animator.SetFloat("xDir", direction.x);
                animator.SetFloat("yDir", direction.y);
                
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, step);
                
                if ((target - transform.position).sqrMagnitude <= 0.01f)
                {
                    break;
                }
        
                yield return null;
            }
            rB.velocity = Vector2.zero;
            
            animator.SetBool("isMove", false);
        }
    }
}

