using System.Collections;
using Gyemong.GameSystem.Creature.Player;
using UnityEngine;


namespace Gyemong.GameSystem.Creature.Mob
{
    public abstract class Mob : Creature
    {

        protected float detectionRange;

        public float DetectionRange
        {
            get { return detectionRange; }
        }

        public float MeleeAttackRange { get; protected set; }
        public float RangedAttackRange { get; protected set; }
        
        public float DistanceToPlayer =>
            Vector3.Distance(transform.position, PlayerCharacter.Instance.transform.position);

        public Vector3 DirectionToPlayer =>
            (PlayerCharacter.Instance.transform.position - transform.position).normalized;
        
        
        public IEnumerator BackStep(float targetDistance)
        {
            Vector3 playerPosition = PlayerCharacter.Instance.transform.position;
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
            Vector3 playerPosition = PlayerCharacter.Instance.transform.position;
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
            yield return new WaitForSeconds(delay);
            while (elapsedTime < duration)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, elapsedTime / duration);
                rb.MovePosition(newPosition);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        
        
        public void TrackPlayer()
        {
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = PlayerCharacter.Instance.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.position = newPosition;
        }


        public abstract IEnumerator Stun(float stunTime);


        public abstract void StartMob();

    }
}