using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer
{
    public class WandererSwordController : MonoBehaviour
    {
        public Animator animator;
        public Transform wanderer;
        public Vector2 rightOffset = new Vector2(0.3f, 0f);
        public Vector2 leftOffset = new Vector2(-0.3f, 0f);
        public Vector2 upOffset = new Vector2(0f, 0.4f);
        public Vector2 downOffset = new Vector2(0f, -0.2f);

        public SpriteRenderer swordRenderer;
        public SpriteRenderer wandererRenderer;

        private bool isPlaying = false;

        public void PlaySlash(Vector2 dir)
        {
            if (isPlaying) return;

            isPlaying = true;
            gameObject.SetActive(true);

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                transform.localPosition = dir.x > 0 ? (Vector3)rightOffset : (Vector3)leftOffset;
            }
            else
            {
                transform.localPosition = dir.y > 0 ? (Vector3)upOffset : (Vector3)downOffset;
            }

            if (dir.y > 0)
            {
                swordRenderer.sortingOrder = wandererRenderer.sortingOrder - 1;
                swordRenderer.flipY = true;
            }
            else
            {
                swordRenderer.sortingOrder = wandererRenderer.sortingOrder + 1;
            }

            animator.SetFloat("xDir", dir.x);
            animator.SetFloat("yDir", dir.y);

            animator.SetTrigger("Slash");
        }

        public void EndSlash()
        {
            gameObject.SetActive(false);
            animator.SetBool("Slash",false);
            isPlaying = false;

            swordRenderer.flipY = false;
        }
    }
}