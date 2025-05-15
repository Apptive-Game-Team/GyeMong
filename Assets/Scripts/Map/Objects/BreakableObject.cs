using UnityEngine;
using System.Collections;

namespace Map.Objects
{
    public class BreakableObject : MonoBehaviour, IAttackable
    {
        private const float BLINK_DELAY = 0.15f;
        [SerializeField] protected float hp;
        private Color? _originalColor = null;
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
        
        public void OnAttacked(float damage = 0)
        {
            print("BreakableObject Attacked");
            hp -= damage;
            StartCoroutine(Blink());
            if (hp <= 0)
            {
                DestroyEvent();
            }
        }

        public virtual void DestroyEvent()
        {
            Destroy(gameObject);   
        }

        private IEnumerator Blink()
        {
            MaterialController?.SetMaterial(MaterialController.MaterialType.HIT);
            MaterialController?.SetFloat(1);
            if (!_originalColor.HasValue)
                _originalColor = GetComponent<SpriteRenderer>().color;
            ;
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(BLINK_DELAY);
            if (MaterialController?.GetCurrentMaterialType() == MaterialController.MaterialType.HIT)
            {
                MaterialController?.SetFloat(0);
            }

            GetComponent<SpriteRenderer>().color = _originalColor.Value; 
        }
    }
}
