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

        // Follow ���� ������ Cube���� ȣ��
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

        public void LockAt() // ���� ����(����)
        {
            isLocked = true;
        }

        private IEnumerator FadeAndFollow()
        {
            // 1) ���̵��� �ϸ鼭 �÷��̾� �߹� ����
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

            // 2) ���Ŀ� ��� �ε巴�� ����(�����Ǹ� ��ġ ����)
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