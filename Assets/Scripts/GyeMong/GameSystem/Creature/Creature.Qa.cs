#if ARTEL_GAME_FUNCTIONS && (UNITY_EDITOR || DEVELOPMENT_BUILD)
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using UnityEngine;

namespace GyeMong.GameSystem.Creature
{
    public abstract partial class Creature
    {
        protected bool QaDamageInProgress { get; private set; }

        // Preserve phase/death dispatch while bypassing defenses only for this call.
        internal void ApplyQaHpDamage(float amount)
        {
            if (currentHp <= 0 || !isActiveAndEnabled) return;
            float shield = currentShield;
            int phase = this is Boss boss ? boss.CurrentPhase : -1;
            currentShield = 0;
            QaDamageInProgress = true;
            try { OnAttacked(Mathf.Min(amount, currentHp)); }
            finally
            {
                QaDamageInProgress = false;
                if (currentHp > 0 && (!(this is Boss b) || b.CurrentPhase == phase))
                    currentShield = shield;
            }
        }
    }
}
#endif
