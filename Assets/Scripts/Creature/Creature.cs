using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


