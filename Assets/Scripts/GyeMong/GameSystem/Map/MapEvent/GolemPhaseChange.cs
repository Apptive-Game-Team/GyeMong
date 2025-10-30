using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class GolemPhaseChange : MonoBehaviour
    {
        [SerializeField] private GolemIKController golem;

        [SerializeField] private Creature.Mob.StateMachineMob.Boss.Boss boss;

        private float delayTime = 3f;
        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            golem.DownAnimation();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());

            yield return new WaitForSeconds(delayTime);

            golem.StartIdleAnimation();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            boss.Trigger();

            yield return new HideBossHealthBarEvent().Execute();

            var showHpBar = new ShowBossHealthBarEvent();
            showHpBar.SetBoss(boss);
            yield return showHpBar.Execute();
        }
    }
}

