using GyeMong.EventSystem;
using GyeMong.GameSystem.Creature.Player.Controller;
using GyeMong.GameSystem.Interface;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Player.Component.Collider
{
    public class AttackCollider : MonoBehaviour
    {
        public float attackDamage;
        private PlayerSoundController _soundController;
        private ParticleSystem _particleSystem;
        private ParticleSystem.ShapeModule _shape;
        private void Start()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _shape = _particleSystem.shape;//.GetComponent<ParticleSystem.ShapeModule>();
            var player = SceneContext.Character;
            attackDamage = player.stat.AttackPower;
        }

        public void Init(PlayerSoundController soundController)
        {
            _soundController = soundController;
        }

        public void SetDamage(float damage)
        {
            attackDamage = damage;
        }
  
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
        
            var creature = collision.GetComponent<Creature>();
            if (creature != null)
            {
                _soundController.Trigger(PlayerSoundType.SWORD_ATTACK);
                creature.OnAttacked(attackDamage);
                SceneContext.Character.AttackIncreaseGauge();
            } 
            else
            {
                IAttackable[] attackableObjects = collision.GetComponents<IAttackable>();
                EventObject[] eventObjects = collision.GetComponents<EventObject>();
                if (attackableObjects.Length != 0)
                {
                    if (eventObjects.Length == 0 || CheckAttackableEventObjects(eventObjects))
                    {
                        _soundController.Trigger(PlayerSoundType.SWORD_ATTACK);
                        foreach (IAttackable @object in attackableObjects)
                        {
                            @object.OnAttacked(attackDamage);
                        }
                    }
                }
            }
        }

        private bool CheckAttackableEventObjects(EventObject[] eventObjects)
        {
            foreach (var eventObject in eventObjects)
            {
                if (eventObject.trigger == EventObject.EventTrigger.OnAttacked) return true;
            }
            return false;
        }

        private void SetParticleSystemTexture(Collider2D collision)
        {
            try
            {
                Sprite sprite;
                try
                {
                    sprite = collision.gameObject.GetComponent<SpriteRenderer>().sprite;
                }
                catch (MissingComponentException)
                {
                    sprite = collision.GetComponentInChildren<SpriteRenderer>().sprite;
                }
                _shape.texture = sprite.texture;
            } catch
            {
                Debug.Log("No SpriteRenderer found");
            }
        }
    }
}
