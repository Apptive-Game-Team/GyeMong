using UnityEngine;

namespace Map.Objects
{
    public class BreakableObject : MonoBehaviour, IAttackable
    {
        [SerializeField] protected float hp;
        public void OnAttacked(float damage = 0)
        {
            print("BreakableObject Attacked");
            hp -= damage;
            if (hp <= 0)
            {
                DestroyEvent();
            }
        }

        public virtual void DestroyEvent()
        {
            Destroy(gameObject);   
        }
    }
}
