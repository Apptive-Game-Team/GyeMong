using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob
{
    public abstract class Mob : Creature
    {

        protected float detectionRange;

        public float DetectionRange
        {
            get { return detectionRange; }
        }

        public float maxMeleeAttackRange { get; protected set; }
        public float minMeleeAttackRange { get; protected set; }
        public float RangedAttackRange { get; protected set; }
        
        public float DistanceToPlayer =>
            Vector3.Distance(transform.position, SceneContext.Character.transform.position);

        public Vector3 DirectionToPlayer =>
            (SceneContext.Character.transform.position - transform.position).normalized;
        
        
        public IEnumerator BackStep(float targetDistance)
        {
            Vector3 playerPosition = SceneContext.Character.transform.position;
            float backStepSpeed = 50f;
            Vector3 direction = (transform.position - playerPosition).normalized;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            LayerMask obstacleLayer = LayerMask.GetMask("Wall", "Player");

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);

            int count = 0;
            while (hit.collider != null && count < 36)
            {
                float angle = 10f;
                direction = Quaternion.Euler(0, 0, angle) * direction;
                hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);
                count++;
            }

            if (hit.collider == null)
            {
                Vector3 targetPosition = transform.position + (direction * targetDistance);
                float elapsedTime = 0f;
                float duration = targetDistance / backStepSpeed;

                while (elapsedTime < duration)
                {
                    Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, elapsedTime / duration);
                    rb.MovePosition(newPosition);
                    elapsedTime += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

                rb.MovePosition(targetPosition);
            }
            else
            {
                yield return null;
            }
        }

        protected Vector3 lastRushDirection;

        public IEnumerator RushAttack(float delay)
        {
            float TARGET_OFFSET = 1f;
            Vector3 playerPosition = SceneContext.Character.transform.position;
            Vector3 direction = (playerPosition - transform.position).normalized;
            lastRushDirection = direction;

            Vector3 targetPosition = playerPosition - (direction * TARGET_OFFSET);
            float targetDistance = Vector3.Distance(transform.position, targetPosition);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            LayerMask obstacleLayer = LayerMask.GetMask("Wall");

            // 충돌 체크
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);
            if (hit.collider != null)
            {
                yield break;
            }

            // 돌진 전 대기
            yield return new WaitForSeconds(delay);

            float currentSpeed = 0f;
            float acceleration = 80f; // 가속도 (값 조절 가능)
            float traveledDistance = 0f;

            while (traveledDistance < targetDistance)
            {
                currentSpeed += acceleration * Time.fixedDeltaTime; // 시간에 따라 속도 증가
                float moveStep = currentSpeed * Time.fixedDeltaTime;

                // 이동량 누적
                traveledDistance += moveStep;

                // 목표 위치를 넘기지 않도록 보정
                if (traveledDistance > targetDistance)
                    moveStep -= (traveledDistance - targetDistance);

                Vector3 newPosition = transform.position + direction * moveStep;
                rb.MovePosition(newPosition);

                yield return new WaitForFixedUpdate();
            }
        }
        public IEnumerator QuickRushAttack()
        {
            float TARGET_OFFSET = 1f;
            Vector3 playerPosition = SceneContext.Character.transform.position;
            float chargeSpeed = 50f;
            Vector3 direction = (playerPosition - transform.position).normalized;
            lastRushDirection = direction;
            Vector3 targetPosition = playerPosition - (direction * TARGET_OFFSET);
            float targetDistance = Vector3.Distance(transform.position, targetPosition);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            LayerMask obstacleLayer = LayerMask.GetMask("Wall");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);

            if (hit.collider != null)
            {
                yield break;
            }

            float elapsedTime = 0f;
            float duration = targetDistance / chargeSpeed;
            while (elapsedTime < duration)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, elapsedTime / duration);
                rb.MovePosition(newPosition);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        public IEnumerator QuickHalfRushAttack()
        {
            float TARGET_OFFSET = 1f;
            Vector3 playerPosition = SceneContext.Character.transform.position;
            float chargeSpeed = 50f;
            Vector3 direction = (playerPosition - transform.position).normalized;
            lastRushDirection = direction;
            Vector3 targetPosition = playerPosition - (direction * TARGET_OFFSET);
            float targetDistance = Vector3.Distance(transform.position, targetPosition);
            float halfDistance = targetDistance / 2f;
            Vector3 halfTargetPosition = transform.position + direction * halfDistance;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            LayerMask obstacleLayer = LayerMask.GetMask("Wall");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, halfDistance, obstacleLayer);
            if (hit.collider != null)
            {
                yield break;
            }
            float elapsedTime = 0f;
            float duration = halfDistance / chargeSpeed;
            while (elapsedTime < duration)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, halfTargetPosition, elapsedTime / duration);
                rb.MovePosition(newPosition);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            rb.MovePosition(halfTargetPosition);
        }

        public void TrackPlayer()
        {
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = SceneContext.Character.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.position = newPosition;
        }


        public abstract IEnumerator Stun(float stunTime);


        public abstract void StartMob();

    }
}