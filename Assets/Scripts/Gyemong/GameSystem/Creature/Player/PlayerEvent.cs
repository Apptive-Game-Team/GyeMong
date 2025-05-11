using System;

namespace Gyemong.GameSystem.Creature.Player
{
    public static class PlayerEvent
    {
        public static event Action<float> OnTakeDamage;
        public static event Action<float> OnHeal;

        public static void TriggerOnTakeDamage(float amount)
        {
            OnTakeDamage?.Invoke(amount);
        }

        public static void TriggerOnHeal(float amount)
        {
            OnHeal?.Invoke(amount);
        }
    }
}

