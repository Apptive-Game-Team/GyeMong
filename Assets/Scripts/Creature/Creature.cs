using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using playerCharacter;
using UnityEngine;
using static Creature.Mob.Boss.Boss;
using Random = UnityEngine.Random;

public enum DirectionType
{
    FRONT,
    BACK,
    LEFT,
    RIGHT
}

namespace Creature
{
    public abstract class Creature : MonoBehaviour, IAttackable
    {
        private const float BLINK_DELAY = 0.15f;

        protected float maxHp;
        [SerializeField] protected float currentHp;

        public float CurrentHp
        {
            get { return currentHp; }
        }

        public float currentShield;

        public float CurrentShield
        {
            get { return currentShield; }
        }

        public float damage;

        protected internal float speed;
        protected float detectionRange;

        public float DetectionRange
        {
            get { return detectionRange; }
        }

        public float MeleeAttackRange { get; protected set; }
        public float RangedAttackRange { get; protected set; }

        protected Coroutine _currentStateCoroutine;

        protected Animator _animator;
        private MaterialController _materialController;

        public MaterialController MaterialController
        {
            get
            {
                if (_materialController == null)
                {
                    _materialController = GetComponent<MaterialController>();
                }

                return _materialController;
            }
        }

        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }

                return _animator;
            }
        }

        public float DistanceToPlayer =>
            Vector3.Distance(transform.position, PlayerCharacter.Instance.transform.position);

        public Vector3 DirectionToPlayer =>
            (PlayerCharacter.Instance.transform.position - transform.position).normalized;

        public DirectionType GetDirectionToPlayer(Vector2 directionToPlayer)
        {
            directionToPlayer.Normalize();
            if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
            {
                return directionToPlayer.x > 0 ? DirectionType.RIGHT : DirectionType.LEFT;
            }
            else
            {
                return directionToPlayer.y > 0 ? DirectionType.FRONT : DirectionType.BACK;
            }
        }


        private Color? _originalColor = null;

        protected IEnumerator Blink()
        {
            MaterialController.SetMaterial(MaterialController.MaterialType.HIT);
            MaterialController.SetFloat(1);
            if (!_originalColor.HasValue)
                _originalColor = GetComponent<SpriteRenderer>().color;
            ;
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(BLINK_DELAY);
            if (MaterialController.GetCurrentMaterialType() == MaterialController.MaterialType.HIT)
            {
                MaterialController.SetFloat(0);
            }

            GetComponent<SpriteRenderer>().color = _originalColor.Value;
        }



        public void TrackPlayer()
        {
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = PlayerCharacter.Instance.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.position = newPosition;
        }

        public void TrackPath(List<Vector2> path)
        {
            if (path == null || path.Count == 0)
            {
                return;
            }

            Vector2 currentTarget = path[0];

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(currentTarget.x, currentTarget.y, currentPosition.z);

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);

            if (Vector3.Distance(currentPosition, targetPosition) < 0.1f)
            {
                path.RemoveAt(0);
            }
        }

        public IEnumerator BackStep(float targetDistance)
        {
            Vector3 playerPosition = PlayerCharacter.Instance.transform.position;
            float backStepSpeed = 50f;
            Vector3 direction = (transform.position - playerPosition).normalized;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");

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

        protected Vector3 lastRushDirection; //�뽬 ���� ���� ����...�� ����� ������?

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
            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
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



        public virtual void OnAttacked(float damage)
        {
            if (currentShield >= damage)
            {
                currentShield -= damage;
            }
            else
            {
                float temp = currentShield;
                currentShield = 0;
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                StartCoroutine(Blink());
                currentHp -= (damage - temp);
            }
        }
        
        protected virtual void OnDead()
        {
        } 
    }
}


