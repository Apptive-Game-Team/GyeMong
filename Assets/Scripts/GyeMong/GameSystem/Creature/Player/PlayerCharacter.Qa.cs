#if ARTEL_GAME_FUNCTIONS && (UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_WEBGL))
using System.Collections.Generic;
using GyeMong.InputSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Player
{
    public partial class PlayerCharacter
    {
        private bool _qaInputActive;
        private readonly HashSet<ActionCode> _qaHeld = new HashSet<ActionCode>();
        private readonly HashSet<ActionCode> _qaPressed = new HashSet<ActionCode>();
        private Vector2 _qaAim;
        internal bool QaIsHealing => isHealing;

        internal bool QaActionAllowed(ActionCode action)
        {
            var input = FindObjectOfType<InputManager>();
            if (input == null || !input.GetKeyActions().ContainsKey(action)) return false;
            try { return input.GetKeyActive(action); }
            catch (KeyNotFoundException) { return false; }
        }

        internal void ApplyQaInput(Vector2 move, Vector2 aim, bool attack, bool skill, bool dash, bool heal)
        {
            _qaInputActive = true;
            _qaAim = aim;
            _qaPressed.Clear();
            SetQaKey(ActionCode.MoveRight, move.x > 0.25f);
            SetQaKey(ActionCode.MoveLeft, move.x < -0.25f);
            SetQaKey(ActionCode.MoveUp, move.y > 0.25f);
            SetQaKey(ActionCode.MoveDown, move.y < -0.25f);
            SetQaKey(ActionCode.Attack, attack);
            SetQaKey(ActionCode.Skill, skill);
            SetQaKey(ActionCode.Dash, dash);
            SetQaKey(ActionCode.Heal, heal);
        }

        private void SetQaKey(ActionCode action, bool held)
        {
            if (held)
            {
                if (_qaHeld.Add(action)) _qaPressed.Add(action);
            }
            else _qaHeld.Remove(action);
        }

        internal void ReleaseQaInput()
        {
            _qaInputActive = false;
            _qaHeld.Clear();
            _qaPressed.Clear();
            comboQueued = false;
            movement = Vector2.zero;
            if (playerRb != null && !isDashing && !isControlled) playerRb.velocity = Vector2.zero;
        }

        internal void SetQaResources(float healthPercent, float gaugePercent, float shield)
        {
            curHealth = stat.HealthMax * Mathf.Clamp(healthPercent, 1, 100) / 100f;
            curSkillGauge = stat.GrazeMax * Mathf.Clamp(gaugePercent, 0, 100) / 100f;
            curShield = Mathf.Max(0, shield);
            changeListenerCaller.CallHpChangeListeners(curHealth);
            changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
            changeListenerCaller.CallShieldChangeListeners(curShield);
        }
    }
}
#endif
