using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public class CubeShadow : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.08f;

        private Transform player;
        private SpriteRenderer sr;
        private Vector3 offset;
        private Vector3 vel = Vector3.zero;

        private float fadeInTime;
        private float startAlpha;
        private float endAlpha;

        private bool isLocked = false;
        private Vector3 lockedPos;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        // Follow 시작 시점에 Cube에서 호출
        public void Initialize(Transform playerTransform, Vector3 followOffset, float fadeInTime, float startAlpha, float endAlpha)
        {
            this.player = playerTransform;
            this.offset = followOffset;
            this.fadeInTime = fadeInTime;
            this.startAlpha = startAlpha;
            this.endAlpha = endAlpha;

            if (sr != null)
            {
                var c = sr.color;
                c.a = startAlpha;
                sr.color = c;
            }

            StartCoroutine(FadeAndFollow());
        }

        public void LockAt(Vector3 worldPos) // 낙하 직전(선택)
        {
            isLocked = true;
            lockedPos = worldPos;
            transform.position = lockedPos;
        }

        private IEnumerator FadeAndFollow()
        {
            // 1) 페이드인 하면서 플레이어 발밑 추적
            float t = 0f;
            while (t < fadeInTime)
            {
                if (!isLocked && player != null)
                {
                    Vector3 target = player.position + offset;
                    transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, smoothTime);
                }

                if (sr != null)
                {
                    var c = sr.color;
                    c.a = Mathf.Lerp(startAlpha, endAlpha, fadeInTime <= 0f ? 1f : t / fadeInTime);
                    sr.color = c;
                }

                t += Time.deltaTime;
                yield return null;
            }

            // 2) 이후엔 계속 부드럽게 추적(고정되면 위치 유지)
            for (; ; )
            {
                if (!isLocked && player != null)
                {
                    Vector3 target = player.position + offset;
                    transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, smoothTime);
                }
                yield return null;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Cube>() != null)
            {
                Destroy(gameObject);
            }
        }
    }

}