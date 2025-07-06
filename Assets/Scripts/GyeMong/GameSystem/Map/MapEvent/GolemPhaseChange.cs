using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class GolemPhaseChange : MonoBehaviour
    {
        [SerializeField] private Animator someAnimator;

        [SerializeField] private Creature.Mob.StateMachineMob.Boss.Boss boss;

        private float delayTime = 3f;
        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            var animBoolEvent = new SetAnimatorParameterBool();
            animBoolEvent.SetAnimator(someAnimator);
            animBoolEvent.SetParameter("isDown", true);
            yield return animBoolEvent.Execute();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());

            yield return new WaitForSeconds(delayTime);

            animBoolEvent.SetAnimator(someAnimator);
            animBoolEvent.SetParameter("isDown", false);
            yield return animBoolEvent.Execute();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            boss.Trigger();

            yield return new HideBossHealthBarEvent().Execute();

            var showHpBar = new ShowBossHealthBarEvent();
            showHpBar.SetBoss(boss);
            yield return showHpBar.Execute();
        }
    }
}

