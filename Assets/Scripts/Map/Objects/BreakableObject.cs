using UnityEngine;

namespace Map.Objects
{
    public class BreakableObject : MonoBehaviour, IAttackable
    {
        [SerializeField] private float hp;
        public void OnAttacked(float damage = 0)
        {
            print("BreakableObject Attacked");
            hp -= damage;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
